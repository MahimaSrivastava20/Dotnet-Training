use Exam_Practice;
Create Table Customer_Master(
	Id INT Primary Key Identity(1, 1),
	Name varchar(50),
	PhoneNo Varchar(10),
	City Varchar(50)
);

Insert Into Customer_Master (Name, PhoneNo, City) Values
	('Ravi Kumar', '9876543210', 'Chennai'),
	('Priya Sharma', '9123456789', 'Bangalore'),
	('John Peter', '9988776655', 'Hyderabad'),
	('Aryan Narayan', '9934762470', 'Patna');


	Create Table Product_Master(
	p_Id INT Primary Key,
	p_Name varchar(50),
	p_Price Decimal(10, 2)
);

Insert Into Product_Master (p_Id, p_Name, p_Price) Values
	(10001, 'Watch', 800),
	(10002, 'Shoes', 1200),
	(10003, 'Redmi Note 10', 15000),
	(10004, 'Laptop', 55000),
	(10005, 'Mouse', 500),
	(10006, 'Keyboard', 1500);


	Create Table SalesPerson_Master(
	Id INT Primary Key Identity(1, 1),
	Name varchar(50),
	PhoneNo Varchar(10),
	City Varchar(50)
);


Insert Into SalesPerson_Master(Name, PhoneNo, City) Values
	('Sonu Kumar', '9876543765', 'Chennai'),
	('Anitha', '8823456789', 'Bangalore'),
	('Suresh', '9188778855', 'Hyderabad'),
	('Mohit Kumar', '7004762470', 'Patna');

Create Table Sales_Details(
	OrderID INT PRIMARY KEY,
	OrderDate Date,
	Customer_Id INT,
	Product_Id INT,
	Quantities INT,
	SalesPerson INT,
	Total Decimal(12,2),
	Foreign Key(Customer_Id) REFERENCES Customer_Master(Id),
	Foreign Key(Product_Id) References Product_Master(p_Id),
	Foreign Key(SalesPerson) References SalesPerson_Master(Id)
);


INSERT INTO Sales_Details(OrderID, OrderDate, Customer_Id, Product_Id, Quantities, SalesPerson) VALUES
	(101, '2024-01-05', 1, 10004, 1, 2),
	(102, '2024-01-05', 1, 10005, 2, 2),
	(103, '2024-01-06', 2, 10006, 1, 2),
	(104, '2024-01-05', 2, 10005, 1, 2),
	(105, '2024-01-10', 1, 10004, 1, 3),
	(106, '2024-02-01', 3, 10003, 1, 2),
	(107, '2024-02-01', 3, 10005, 1, 2),
	(108, '2024-01-02', 4, 10004, 1, 4),
	(109, '2024-03-01', 4, 10002, 2, 1);



Select * From Customer_Master;
Select * From Product_Master;
Select * From SalesPerson_Master;
Select * From Sales_Details;

Update sd SET Total = p.p_Price * sd.Quantities  FROM Sales_Details sd inner join Product_Master p on p.p_Id = sd.Product_Id;



-- Q.2 Write a SQL query to find the third highest total sales amount
Select * From Sales_Details Order by Total desc OFFSET 1 ROWS Fetch NEXT 1 ROW ONLY;


-- Q.3 Write a query to list SalesPerson names whose total sales amount is greater than ?60,000.
Select sp.Name, SUM(sd.Total) AS Total_Sales_Amount from Sales_Details sd 
inner join SalesPerson_Master sp on sd.SalesPerson = sp.Id Group by sp.Name HAVING SUM(sd.Total) > 60000;


/* Q.5 
Operations team wants formatted and filtered data.
Tasks:
Display CustomerName in UPPERCASE
Extract Order Month from OrderDate
Show only orders placed in January 2024
*/

Select DISTINCT UPPER(cm.Name) AS CustomerName, DATENAME(MONTH, sd.OrderDate) AS OrderMonth from Sales_Details
sd inner join Customer_Master cm on cm.Id = sd.Customer_Id where Month(sd.OrderDate) = 1 And YEAR(sd.OrderDate) = 2024;



