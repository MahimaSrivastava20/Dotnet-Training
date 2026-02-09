use TrainingInstitute



CREATE TABLE Student (
    StudentId INT PRIMARY KEY,
    StudentName NVARCHAR(100),
    JoiningDate DATE
);



CREATE TABLE Trainer (
    TrainerId INT PRIMARY KEY IDENTITY,
    TrainerName NVARCHAR(100)
);

CREATE TABLE Course (
    CourseId INT PRIMARY KEY IDENTITY,
    CourseName NVARCHAR(100),
    CourseFee DECIMAL(10,2),
    TrainerId INT,
    FOREIGN KEY (TrainerId) REFERENCES Trainer(TrainerId)
);

CREATE TABLE Marks (
    MarksId INT PRIMARY KEY IDENTITY,
    StudentId INT,
    CourseId INT,
    ExamMonth INT,
    ExamYear INT,
    Marks INT,
    FOREIGN KEY (StudentId) REFERENCES Student(StudentId),
    FOREIGN KEY (CourseId) REFERENCES Course(CourseId)
);

INSERT INTO Trainer (TrainerName)
VALUES 
('Mr. Mari'),
('Mr. Pankaj'),
('Mr. Gopi'),
('Mr. Ravi');


INSERT INTO Student (StudentId, StudentName, JoiningDate)
VALUES
(101,'Mahima','2021-06-15'),
(102,'Mansi', '2022-01-10'),
(103,'Devashish','2020-07-20'),
(104,'Nishu','2023-03-05');


INSERT INTO Course (CourseName, CourseFee, TrainerId)
VALUES
('SQL', 18000, 1),
('Java', 22000, 2),
('Dotnet', 28000, 3);



INSERT INTO Marks (StudentId, CourseId, ExamMonth, ExamYear, Marks)
VALUES
(101, 1, 1,  YEAR(GETDATE()), 78), 
(101, 2, 3,  YEAR(GETDATE()), 85),  
(102, 1, 5,  YEAR(GETDATE()), 62),  
(103, 3, 7,  YEAR(GETDATE()), 91),   
(104, 2, 9,  YEAR(GETDATE()), 38),   
(102, 3, 11, YEAR(GETDATE()), 44);   

select * from [dbo].[Student]
select * from [dbo].[Trainer]
select * from [dbo].[Course]
select * from [dbo].[Marks]


-----------------------------------------------------------------------

--Question2---> ALTER TABLE (Add RewardPoints)
alter table Student
add RewardPoints INT DEFAULT 0;


--Question3----> CHECK Constraint
alter table  Student
add constraint Check_RewardPoints
CHECK (RewardPoints BETWEEN 0 AND 100);  --done till here--


--Question4----> INNER JOIN (Students Who Appeared for Exams)
SELECT 
s.StudentName,
c.CourseName,
t.TrainerName,
CONCAT(m.ExamMonth, '-', m.ExamYear) AS ExamPeriod,
m.Marks
FROM Marks m
INNER JOIN Student s ON m.StudentId = s.StudentId
INNER JOIN Course c ON m.CourseId = c.CourseId
INNER JOIN Trainer t ON c.TrainerId = t.TrainerId;


--Question5-----> Total Marks in Current Year
SELECT 
s.StudentName,
SUM(m.Marks) AS TotalMarks
FROM Student s
INNER JOIN Marks m ON s.StudentId = m.StudentId
WHERE m.ExamYear = YEAR(GETDATE())
GROUP BY s.StudentName;


--Question6-----> SUBSTRING + LEFT (Login ID)

SELECT 
s.StudentName,
substring(s.StudentName, 1, 3) +
left(c.CourseName, 2) +
cast(s.StudentId AS VARCHAR) AS LoginID
FROM Student s
INNER JOIN Marks m ON s.StudentId = m.StudentId
INNER JOIN Course c ON m.CourseId = c.CourseId;


--Question7----->  Subquery (Above Average Marks)
 
 select AVG(Marks) from Marks --63


SELECT DISTINCT s.StudentName
FROM Student s
INNER JOIN Marks m ON s.StudentId = m.StudentId
WHERE m.Marks >
(
    select AVG(Marks) FROM Marks
);


--Question8---->  UNION(HIGH & LOW Scorers)
select 
s.StudentName,
m.Marks,
'HIGH' AS Category
FROM Student s
INNER JOIN Marks m ON s.StudentId = m.StudentId
WHERE m.Marks > 80 UNION
select
s.StudentName, m.Marks, 'LOW' AS Category FROM Student s
INNER JOIN Marks m ON s.StudentId = m.StudentId WHERE m.Marks < 40;



--Question10---> COALESCE + Date Calculation (Scholarship)

SELECT 
s.StudentName,
s.JoiningDate,
DATEDIFF(YEAR, s.JoiningDate, GETDATE()) AS YearsOfStudy,
case
when DATEDIFF(YEAR, s.JoiningDate, GETDATE()) >= 3
then COALESCE(10000, 0)
else COALESCE(0, 0)
end as ScholarshipAmount
from Student s;




--Question9-----> Trigger (Update RewardPoints)

create TRIGGER trg_UpdateRewardPointtss
ON Marks
AFTER INSERT
AS
BEGIN
    UPDATE s
    SET RewardPoints = r.TotalPoints
    FROM Student s
    INNER JOIN (SELECT StudentId,
SUM(
CASE
WHEN Marks >= 80 THEN 10
WHEN Marks >= 60 THEN 5
END
) AS TotalPoints
FROM Marks
GROUP BY StudentId
) r ON s.StudentId = r.StudentId;
END;


SELECT StudentId, StudentName, RewardPoints
FROM Student;






















SELECT StudentId, COUNT(*) AS TotalExams
FROM Marks
WHERE StudentId = 101
GROUP BY StudentId;





