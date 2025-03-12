-- Tạo cơ sở dữ liệu
CREATE DATABASE OnlineStore2;
USE OnlineStore2;

-- Bảng Users
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Phone NVARCHAR(15),
    Address NVARCHAR(255),
    Role NVARCHAR(20) DEFAULT 'Customer',
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Bảng Categories
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255)
);

-- Bảng Products
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    Price DECIMAL(18, 2) NOT NULL,
    StockQuantity INT NOT NULL,
    CategoryID INT,
    ImageURL NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);

-- Bảng Orders
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18, 2) NOT NULL,
    OrderStatus NVARCHAR(20) DEFAULT 'Pending',
    PaymentStatus NVARCHAR(20) DEFAULT 'Unpaid',
    ShippingStatus NVARCHAR(20) DEFAULT 'Not Shipped',
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Bảng OrderDetails
CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT,
    ProductID INT,
    Quantity INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- Bảng ShoppingCart
CREATE TABLE ShoppingCart (
    CartID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT,
    ProductID INT,
    Quantity INT NOT NULL,
    AddedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- Bảng Payments
CREATE TABLE Payments (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT,
    PaymentDate DATETIME DEFAULT GETDATE(),
    PaymentMethod NVARCHAR(50),
    Amount DECIMAL(18, 2) NOT NULL,
    PaymentStatus NVARCHAR(20) DEFAULT 'Pending',
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);

-- Bảng Shipping
CREATE TABLE Shipping (
    ShippingID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT,
    ShippingAddress NVARCHAR(255) NOT NULL,
    ShippingMethod NVARCHAR(50) NOT NULL,
    TrackingNumber NVARCHAR(50),
    EstimatedDeliveryDate DATE,
    ShippingStatus NVARCHAR(20) DEFAULT 'Not Shipped',
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);

-- Bảng ProductReviews
CREATE TABLE ProductReviews (
    ReviewID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT,
    UserID INT,
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(500),
    ReviewDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Bảng ProductRepairs
CREATE TABLE ProductRepairs (
    RepairID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT,
    UserID INT,
    RepairRequestDate DATETIME DEFAULT GETDATE(),
    IssueDescription NVARCHAR(500),
    RepairStatus NVARCHAR(50) DEFAULT 'Pending',
    RepairCompletionDate DATETIME,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Bảng Discounts
CREATE TABLE Discounts (
    DiscountID INT IDENTITY(1,1) PRIMARY KEY,
    DiscountCode NVARCHAR(50) UNIQUE NOT NULL,
    Description NVARCHAR(255),
    DiscountPercentage DECIMAL(5, 2) CHECK (DiscountPercentage BETWEEN 0 AND 100),
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    IsActive BIT DEFAULT 1
);

-- Bảng ProductDiscounts
CREATE TABLE ProductDiscounts (
    ProductDiscountID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT,
    DiscountID INT,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (DiscountID) REFERENCES Discounts(DiscountID)
);

-- Tạo chỉ mục để tối ưu truy vấn
CREATE INDEX IX_Products_CategoryID ON Products(CategoryID);
CREATE INDEX IX_Orders_UserID ON Orders(UserID);
CREATE INDEX IX_ShoppingCart_UserID ON ShoppingCart(UserID);
CREATE INDEX IX_ProductReviews_ProductID ON ProductReviews(ProductID);
CREATE INDEX IX_ProductReviews_UserID ON ProductReviews(UserID);
CREATE INDEX IX_ProductRepairs_ProductID ON ProductRepairs(ProductID);
CREATE INDEX IX_ProductRepairs_UserID ON ProductRepairs(UserID);
CREATE INDEX IX_ProductDiscounts_ProductID ON ProductDiscounts(ProductID);
CREATE INDEX IX_ProductDiscounts_DiscountID ON ProductDiscounts(DiscountID);

-- Kiểm tra dữ liệu
SELECT * FROM Users;
SELECT * FROM Products;
SELECT * FROM Orders;
SELECT * FROM OrderDetails;
SELECT * FROM ShoppingCart;
SELECT * FROM Payments;
SELECT * FROM Shipping;
SELECT * FROM ProductReviews;
SELECT * FROM ProductRepairs;
SELECT * FROM Discounts;
SELECT * FROM ProductDiscounts;

-- Mô tả các quan hệ chính
-- 1. Một người dùng có thể có nhiều đơn hàng.
-- 2. Một đơn hàng có thể có nhiều sản phẩm thông qua OrderDetails.
-- 3. Mỗi sản phẩm thuộc về một danh mục duy nhất.
-- 4. Mỗi đơn hàng có một bản ghi thanh toán và một bản ghi giao hàng.
-- 5. Giỏ hàng chứa các sản phẩm mà người dùng chưa đặt hàng.
-- 6. Người dùng có thể đánh giá sản phẩm thông qua ProductReviews.
-- 7. Người dùng có thể yêu cầu sửa chữa sản phẩm thông qua ProductRepairs.
-- 8. Sản phẩm có thể được áp dụng nhiều mã giảm giá.
