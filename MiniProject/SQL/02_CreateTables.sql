USE EHR;
GO

-- Table: Patient
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Healthcare].[Patient]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Healthcare].[Patient](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [DateOfBirth] [date] NOT NULL,
        [Gender] [nvarchar](10) NOT NULL,
        [Email] [nvarchar](100) NULL,
        [Phone] [nvarchar](20) NULL,
        CONSTRAINT [PK_Patient] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- Table: Appointment
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Healthcare].[Appointment]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Healthcare].[Appointment](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [PatientId] [int] NOT NULL,
        [AppointmentDate] [datetime2](7) NOT NULL,
        [Reason] [nvarchar](255) NULL,
        [Status] [nvarchar](50) NOT NULL DEFAULT 'Scheduled', -- Scheduled, Completed, Cancelled
        [DoctorName] [nvarchar](100) NULL,
        [DoctorId] [int] NULL,
        CONSTRAINT [PK_Appointment] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Appointment_Patient] FOREIGN KEY([PatientId]) REFERENCES [Healthcare].[Patient] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Appointment_Doctor] FOREIGN KEY([DoctorId]) REFERENCES [Healthcare].[Doctor] ([Id])
    );
END
GO

-- Table: LabOrder
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Healthcare].[LabOrder]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Healthcare].[LabOrder](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [AppointmentId] [int] NOT NULL,
        [TestName] [nvarchar](100) NOT NULL,
        [OrderDate] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [Status] [nvarchar](50) NOT NULL DEFAULT 'Pending', -- Pending, Completed
        [Result] [nvarchar](max) NULL,
        CONSTRAINT [PK_LabOrder] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_LabOrder_Appointment] FOREIGN KEY([AppointmentId]) REFERENCES [Healthcare].[Appointment] ([Id]) ON DELETE CASCADE
    );
END
GO

-- Table: AuditLog
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Healthcare].[AuditLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Healthcare].[AuditLog](
        [LogId] [int] IDENTITY(1,1) NOT NULL,
        [TableName] [nvarchar](50) NOT NULL,
        [RecordId] [int] NOT NULL,
        [Action] [nvarchar](50) NOT NULL, -- INSERT, UPDATE, DELETE
        [LogDate] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [Details] [nvarchar](max) NULL,
        CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED ([LogId] ASC)
    );
END
GO
