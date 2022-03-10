
        
CREATE TABLE Catalog
(
  ID          INTEGER      NOT NULL,
  model       VARCHAR(255) NOT NULL,
  color       INTEGER      NOT NULL,
  size        INTEGER      NOT NULL,
  price       INTEGER      NULL    ,
  description VARCHAR(255) NULL    ,
  image       BLOB         NULL    ,
  PRIMARY KEY (ID)
);

CREATE TABLE Colors
(
  ID          INTEGER      NOT NULL,
  description VARCHAR(255) NULL    ,
  PRIMARY KEY (ID)
);

CREATE TABLE componentlink
(
  part_number INTEGER NOT NULL,
  ID          INTEGER NOT NULL,
  amount      INTEGER NULL    
);

CREATE TABLE Components
(
  part_number INTEGER      NOT NULL,
  in_stock    INTEGER      NULL    ,
  description VARCHAR(255) NULL    ,
  location    VARCHAR(255) NULL    ,
  PRIMARY KEY (part_number)
);

CREATE TABLE customers
(
  customer_number INTEGER      NOT NULL,
  firstname       VARCHAR(255) NULL    ,
  lastname        VARCHAR(255) NULL    ,
  address         VARCHAR(255) NULL    ,
  phone           VARCHAR(255) NULL    ,
  email           VARCHAR(255) NULL    ,
  PRIMARY KEY (customer_number)
);

CREATE TABLE invoice_details
(
  invoice_number INTEGER NOT NULL,
  ID             INTEGER NOT NULL,
  amount         INTEGER NULL    ,
  price          INTEGER NULL    
);

CREATE TABLE invoices
(
  invoice_number  INTEGER NOT NULL,
  customer_number INTEGER NOT NULL,
  date            DATE    NULL    ,
  totalPrice      INTEGER NULL    ,
  PRIMARY KEY (invoice_number)
);

ALTER TABLE invoices
  ADD CONSTRAINT FK_customers_TO_invoices
    FOREIGN KEY (customer_number)
    REFERENCES customers (customer_number);

ALTER TABLE invoice_details
  ADD CONSTRAINT FK_invoices_TO_invoice_details
    FOREIGN KEY (invoice_number)
    REFERENCES invoices (invoice_number);

ALTER TABLE invoice_details
  ADD CONSTRAINT FK_Catalog_TO_invoice_details
    FOREIGN KEY (ID)
    REFERENCES Catalog (ID);

ALTER TABLE componentlink
  ADD CONSTRAINT FK_Components_TO_componentlink
    FOREIGN KEY (part_number)
    REFERENCES Components (part_number);

ALTER TABLE componentlink
  ADD CONSTRAINT FK_Catalog_TO_componentlink
    FOREIGN KEY (ID)
    REFERENCES Catalog (ID);

        
      