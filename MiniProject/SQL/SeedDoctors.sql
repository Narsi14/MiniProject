-- 1. Create Doctor Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Doctor' AND schema_id = SCHEMA_ID('Healthcare'))
BEGIN
    CREATE TABLE Healthcare.Doctor (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Specialization NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        PhoneNumber NVARCHAR(20) NOT NULL
    );
END
GO

-- 2. DoctorId is already added in the main 02_CreateTables.sql script
GO

-- 3. Seed Dummy Data for Doctors
IF NOT EXISTS (SELECT * FROM Healthcare.Doctor)
BEGIN
    INSERT INTO Healthcare.Doctor (Name, Specialization, Email, PhoneNumber)
    VALUES 
    ('Dr. John Smith', 'Cardiology', 'john.smith@hospital.com', '555-0101'),
    ('Dr. Sarah Johnson', 'Pediatrics', 'sarah.johnson@hospital.com', '555-0102'),
    ('Dr. Michael Brown', 'Neurology', 'michael.brown@hospital.com', '555-0103'),
    ('Dr. Emily Davis', 'General Practice', 'emily.davis@hospital.com', '555-0104'),
    ('Dr. David Wilson', 'Orthopedics', 'david.wilson@hospital.com', '555-0105');
END
GO
