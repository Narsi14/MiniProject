-- Fix for INSTEAD OF TRIGGER limitation with CASCADE DELETE
-- We must remove CASCADE DELETE from incoming and outgoing relationships if they conflict.
-- Specifically, Appointment is a child of Patient with Cascade Delete. This blocks INSTEAD OF DELETE.
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Healthcare].[FK_Appointment_Patient]') AND parent_object_id = OBJECT_ID(N'[Healthcare].[Appointment]'))
BEGIN
    ALTER TABLE [Healthcare].[Appointment] DROP CONSTRAINT [FK_Appointment_Patient];
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Healthcare].[FK_Appointment_Patient]') AND parent_object_id = OBJECT_ID(N'[Healthcare].[Appointment]'))
BEGIN
    ALTER TABLE [Healthcare].[Appointment]  WITH CHECK ADD  CONSTRAINT [FK_Appointment_Patient] FOREIGN KEY([PatientId])
    REFERENCES [Healthcare].[Patient] ([Id])
    -- ON DELETE CASCADE REMOVED
END

-- Also handling LabOrder (Child) just in case, though the Parent restriction is less strict, standardizing on NO ACTION is better for this logic.
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Healthcare].[FK_LabOrder_Appointment]') AND parent_object_id = OBJECT_ID(N'[Healthcare].[LabOrder]'))
BEGIN
    ALTER TABLE [Healthcare].[LabOrder] DROP CONSTRAINT [FK_LabOrder_Appointment];
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Healthcare].[FK_LabOrder_Appointment]') AND parent_object_id = OBJECT_ID(N'[Healthcare].[LabOrder]'))
BEGIN
    ALTER TABLE [Healthcare].[LabOrder]  WITH CHECK ADD  CONSTRAINT [FK_LabOrder_Appointment] FOREIGN KEY([AppointmentId])
    REFERENCES [Healthcare].[Appointment] ([Id])
    -- ON DELETE CASCADE REMOVED
END
GO

-- Trigger for LabOrder: Deletion only if status is 'Completed'
CREATE OR ALTER TRIGGER [Healthcare].[trg_LabOrder_DeleteControl]
ON [Healthcare].[LabOrder]
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Block deletion if any order is 'Pending'
    IF EXISTS (SELECT 1 FROM deleted WHERE Status = 'Pending')
    BEGIN
        INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Action, Details, LogDate)
        SELECT 'LabOrder', Id, 'DELETE BLOCKED', 'Attempted to delete a Pending lab order', GETDATE()
        FROM deleted WHERE Status = 'Pending';
        
        RAISERROR('Cannot delete Lab Order with Pending status.', 16, 1);
        -- Only rollback if it's an explicit delete on LabOrder, not via cascade from Appointment/Patient
        -- However, since CASCADE is off, all deletes are explicit or handled via trigger cascading.
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Perform the actual deletion for 'Completed' orders
    DELETE lo
    FROM [Healthcare].[LabOrder] lo
    INNER JOIN deleted d ON lo.Id = d.Id
    WHERE d.Status = 'Completed';
END
GO

-- Updated Trigger for Appointment: Allow deletion ONLY if no linked LabOrders are 'Pending'
CREATE OR ALTER TRIGGER [Healthcare].[trg_AppointmentAudit_BlockParams]
ON [Healthcare].[Appointment]
INSTEAD OF UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- HANDLE UPDATE
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) 
    BEGIN
         INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Action, Details, LogDate)
         SELECT 
            'Appointment', 
            d.Id, 
            'UPDATE ATTEMPT', 
            CONCAT('Attempted update on Appointment from Status: ', d.Status, ' to ', i.Status),
            GETDATE()
         FROM inserted i
         JOIN deleted d ON i.Id = d.Id;
         
         -- Allow the update to proceed (Optional: remove blocking logic)
         UPDATE a
         SET a.Status = i.Status,
             a.Reason = i.Reason,
             a.AppointmentDate = i.AppointmentDate,
             a.DoctorId = i.DoctorId,
             a.DoctorName = i.DoctorName
         FROM [Healthcare].[Appointment] a
         INNER JOIN inserted i ON a.Id = i.Id;
    END
    -- HANDLE DELETE
    ELSE IF EXISTS (SELECT * FROM deleted) AND NOT EXISTS (SELECT * FROM inserted)
    BEGIN
         -- Check for PENDING LabOrders
         IF EXISTS (
            SELECT 1 FROM [Healthcare].[LabOrder] lo
            JOIN deleted d ON lo.AppointmentId = d.Id
            WHERE lo.Status = 'Pending'
         )
         BEGIN
             INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Action, Details, LogDate)
             SELECT 
                'Appointment', 
                d.Id, 
                'DELETE BLOCKED', 
                'Attempted delete of Appointment which has Pending Lab Orders', 
                GETDATE()
             FROM deleted d;

             RAISERROR('Cannot delete Appointment because it has Pending Lab Orders.', 16, 1);
             ROLLBACK TRANSACTION;
             RETURN;
         END

         -- If we reach here, either no lab orders exist or they are all 'Completed'
         -- First, manually delete linked LabOrders (as CASCADE is removed)
         DELETE lo
         FROM [Healthcare].[LabOrder] lo
         INNER JOIN deleted d ON lo.AppointmentId = d.Id;

         -- Then delete the Appointment
         DELETE a
         FROM [Healthcare].[Appointment] a
         INNER JOIN deleted d ON a.Id = d.Id;

         INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Action, Details, LogDate)
         SELECT 
            'Appointment', 
            d.Id, 
            'DELETE SUCCESS', 
            'Deleted Appointment and its Completed Lab Orders', 
            GETDATE()
         FROM deleted d;
    END
END
GO

-- Trigger for Patient: Deletion only if no linked Appointments have Pending Lab Orders
CREATE OR ALTER TRIGGER [Healthcare].[trg_Patient_DeleteControl]
ON [Healthcare].[Patient]
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Check for PENDING LabOrders in any linked Appointment
    IF EXISTS (
        SELECT 1 FROM [Healthcare].[LabOrder] lo
        JOIN [Healthcare].[Appointment] a ON lo.AppointmentId = a.Id
        JOIN deleted d ON a.PatientId = d.Id
        WHERE lo.Status = 'Pending'
    )
    BEGIN
        INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Action, Details, LogDate)
        SELECT 'Patient', Id, 'DELETE BLOCKED', 'Attempted to delete patient who has appointments with Pending Lab Orders', GETDATE()
        FROM deleted;

        RAISERROR('Cannot delete Patient with Pending Lab Orders in their appointments.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- If allowed, manually cascade the deletion
    -- 1. Delete LabOrders
    DELETE lo
    FROM [Healthcare].[LabOrder] lo
    JOIN [Healthcare].[Appointment] a ON lo.AppointmentId = a.Id
    JOIN deleted d ON a.PatientId = d.Id;

    -- 2. Delete Appointments
    DELETE a
    FROM [Healthcare].[Appointment] a
    JOIN deleted d ON a.PatientId = d.Id;

    -- 3. Delete Patient
    DELETE p
    FROM [Healthcare].[Patient] p
    INNER JOIN deleted d ON p.Id = d.Id;

    INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Action, Details, LogDate)
    SELECT 'Patient', Id, 'DELETE SUCCESS', 'Deleted Patient and all linked records', GETDATE()
    FROM deleted;
END
GO
