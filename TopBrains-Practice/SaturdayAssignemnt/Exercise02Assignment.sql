use [Tabless];


alter table ZIPCODE_INFO
alter column CITY varchar(25);

alter table ZIPCODE_INFO
add state varchar(2);



--------------------------------------------------------

alter table INSTRUCTOR_INFO
alter column INSTRUCTOR_FIRST_NAME varchar(25);

alter table INSTRUCTOR_INFO
alter column INSTRUCTOR_LAST_NAME varchar(25);

alter table INSTRUCTOR_INFO
add STREET_ADDRESS varchar(50),
    ZIP_CODE varchar(5);


 ---------------------------------------------------------

alter table COURSE_INFO
add COURSE_NAME varchar(50),
    COURSE_PREREQUISITE numeric(8,0);

alter table COURSE_INFO
alter column COST numeric(9,2);


------------------------------------------------------------


alter table STUDENT_INFO
alter column STUDENT_FIRST_NAME varchar(25);

alter table STUDENT_INFO
alter column STUDENT_LAST_NAME varchar(25);

alter table STUDENT_INFO
add STREET_ADDRESS varchar(50),
    ZIP_CODE varchar(5);



-------------------------------------------------------------

alter table SECTION_INFO
alter column SECTION_NO numeric(3,0);

alter table SECTION_INFO
add location varchar(50),
    CAPACITY numeric(3,0);



--------------------------------------------------------------

alter table ENROLLMENT_INFO
add ENROLLMENT_DATE date;



---------------------------------------------------------------

alter table GRADE_INFO
alter column GRADE_CODE_OCCURANCE numeric(38,0);

alter table GRADE_INFO
add NUMERIC_GRADE numeric(3,0);

select * from [dbo].[SECTION_INFO]
select * from [dbo].[STUDENT_INFO]

