use [BankDetailsHardProject];

CREATE TABLE Customers
(
    CustomerID INT PRIMARY KEY,
    CustomerName VARCHAR(100),
    PhoneNumber VARCHAR(15),
    City VARCHAR(50),
    CreatedDate DATE
);

CREATE TABLE Accounts
(
    AccountID INT PRIMARY KEY,
    CustomerID INT,
    AccountNumber VARCHAR(20),
    AccountType VARCHAR(20), -- Savings / Current
    OpeningBalance DECIMAL(12,2),
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
);

CREATE TABLE Transactions
(
    TransactionID INT PRIMARY KEY,
    AccountID INT,
    TransactionDate DATE,
    TransactionType VARCHAR(10), -- Deposit / Withdraw
    Amount DECIMAL(12,2),
    FOREIGN KEY (AccountID) REFERENCES Accounts(AccountID)
);


CREATE TABLE Bonus
(
    BonusID INT PRIMARY KEY,
    AccountID INT,
    BonusMonth INT,
    BonusYear INT,
    BonusAmount DECIMAL(10,2),
    CreatedDate DATE,
    FOREIGN KEY (AccountID) REFERENCES Accounts(AccountID)
);


INSERT INTO Customers VALUES
(1, 'Ravi Kumar', '9876543210', 'Chennai', '2023-01-10'),
(2, 'Priya Sharma', '9123456789', 'Bangalore', '2023-03-15'),
(3, 'John Peter', '9988776655', 'Hyderabad', '2023-06-20');


INSERT INTO Accounts VALUES
(101, 1, 'SB1001', 'Savings', 20000),
(102, 2, 'SB1002', 'Savings', 15000),
(103, 3, 'SB1003', 'Savings', 30000);


INSERT INTO Transactions VALUES
(1, 101, '2024-01-05', 'Deposit', 30000),
(2, 101, '2024-01-18', 'Withdraw', 5000),
(3, 101, '2024-02-10', 'Deposit', 25000),
(4, 102, '2024-01-07', 'Deposit', 20000),
(5, 102, '2024-01-25', 'Deposit', 35000),
(6, 102, '2024-02-05', 'Withdraw', 10000),
(7, 103, '2024-01-10', 'Deposit', 15000),
(8, 103, '2024-01-20', 'Withdraw', 5000);

------------
--Question 1
------------
/*Question 1 – Stored Procedure (Date Range + Aggregation)
Write a stored procedure that accepts:
@StartDate
@EndDate
@AccountID

Output:
Total Deposited Amount during the given period
Total Withdrawn Amount during the given period
The procedure should return both values in a single result.*/

CREATE proc usp_GetTransactionSummary
@StartDate DATE,
@EndDate DATE,
@AccountID INT
as
begin
select ISNULL((SELECT SUM(Amount) FROM Transactions WHERE TransactionType = 'Deposit' AND AccountID = @AccountID AND 
TransactionDate BETWEEN @StartDate AND @EndDate), 0) AS TotalDeposited,

ISNULL((SELECT SUM(Amount) FROM Transactions WHERE TransactionType = 'Withdraw' AND AccountID = @AccountID
AND TransactionDate BETWEEN @StartDate AND @EndDate), 0) AS TotalWithdrawn;
END;

EXEC usp_GetTransactionSummary '2024-01-01', '2024-02-28', 101;



------------
--Question 2
------------
insert into Bonus (BonusID, AccountID, BonusMonth, BonusYear, BonusAmount, CreatedDate)
select 
ROW_NUMBER() OVER (ORDER BY AccountID, month(TransactionDate)) + ISNULL((SELECT MAX(BonusID) from Bonus),0),
AccountID, 
MONTH(TransactionDate) AS BonusMonth,
YEAR(TransactionDate) AS BonusYear,
1000 AS BonusAmount,
GETDATE()
from Transactions
where TransactionType = 'Deposit'
group by AccountID, month(TransactionDate), year(TransactionDate)
having sum(Amount) > 50000
AND NOT EXISTS (
SELECT 1 from Bonus b where b.AccountID = Transactions.AccountID AND b.BonusMonth = MONTH(TransactionDate) AND b.BonusYear = YEAR(TransactionDate));

------------
--Question 3
------------
SELECT c.CustomerName, a.AccountNumber, a.OpeningBalance 
+ ISNULL((SELECT SUM(Amount) 
FROM Transactions 
WHERE TransactionType = 'Deposit' 
AND AccountID = a.AccountID), 0)

- ISNULL((SELECT SUM(Amount) 
FROM Transactions 
WHERE TransactionType = 'Withdraw' 
AND AccountID = a.AccountID), 0)
+ ISNULL((SELECT SUM(BonusAmount) FROM Bonus WHERE AccountID = a.AccountID), 0)
AS CurrentBalance
FROM Customers c
JOIN Accounts a 
ON c.CustomerID = a.CustomerID;




