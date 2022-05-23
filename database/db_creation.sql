
        
CREATE TABLE Catalog
(
  ID          INTEGER    NOT NULL,
  model       LINESTRING NOT NULL,
  color       INTEGER    NOT NULL,
  size        INTEGER    NOT NULL,
  description LINESTRING NULL    ,
  image       BLOB       NULL    ,
  PRIMARY KEY (ID)
);

CREATE TABLE Components
(
  part_number INTEGER    NOT NULL,
  ID          INTEGER    NOT NULL,
  amount      INTEGER    NULL    ,
  in_stock    INTEGER    NULL    ,
  description LINESTRING NULL    ,
  location    LINESTRING NULL    ,
  PRIMARY KEY (part_number)
);

CREATE TABLE customers
(
  customer_number INTEGER    NOT NULL,
  firstname       LINESTRING NULL    ,
  lastname        LINESTRING NULL    ,
  address         LINESTRING NULL    ,
  phone           LINESTRING NULL    ,
  email           LINESTRING NULL    ,
  PRIMARY KEY (customer_number)
);

CREATE TABLE invoice_details
(
  invoice_number INTEGER NOT NULL        ,
  invoice_detail_id INT NOT NULL IDENTITY,
  ID             INTEGER NOT NULL        ,
  amount         INTEGER NULL            ,
  price          INTEGER NULL            ,
  status VARCHAR(255) DEFAULT 'waiting'  ,
);

CREATE TABLE invoices
(
  invoice_number  INTEGER NOT NULL,
  customer_number INTEGER NOT NULL,
  date            DATE    NULL    ,
  totalPrice int DEFAULT NULL     ,
  PRIMARY KEY (invoice_number)
);

CREATE TABLE part_orders 
(
  order_number INTEGER NOT NULL IDENTITY,
  part_number INTEGER NOT NULL          ,
  amount DECIMAL(10, 0) NOT NULL        ,
  status VARCHAR(255) DEFAULT NULL      ,
  PRIMARY KEY (order_number)
);

CREATE TABLE production 
(
  production_ID INTEGER NOT NULL IDENTITY,
  invoice_detail_id INTEGER NOT NULL     ,
  ID INTEGER NOT NULL                    ,
  amount INTEGER DEFAULT NULL            ,
  amount_scheduled INT DEFAULT NULL      ,
  amount_completed INT DEFAULT NULL      ,
  PRIMARY KEY (production_ID)            ,
);

CREATE TABLE week_schedule (
  bike_id INT NOT NULL IDENTITY          ,
  ID INT NOT NULL                        ,
  production_id INT NOT NULL             ,
  day VARCHAR(255) NOT NULL              ,
  PRIMARY KEY (bike_id)
);

ALTER TABLE Components
  ADD CONSTRAINT FK_Catalog_TO_Components
    FOREIGN KEY (ID)
    REFERENCES Catalog (ID);

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

ALTER TABLE part_orders
  ADD CONSTRAINT FK_components_TO_orders
    FOREIGN KEY (part_number)
    REFERENCES Components (part_number);

ALTER TABLE production
  ADD CONSTRAINT FK_Catalog_TO_production
    FOREIGN KEY (ID)
    REFERENCES Catalog (ID),
  CONSTRAINT FK_invoice_details_TO_production
    FOREIGN KEY (invoice_detail_id)
    REFERENCES invoices (invoice_detail_id);

ALTER TABLE bovelo.week_schedule
  ADD CONSTRAINT FK_Catalog_week_schedule_ID 
    FOREIGN KEY (ID)
    REFERENCES bovelo.Catalog (ID);

ALTER TABLE bovelo.week_schedule
  ADD CONSTRAINT FK_production_TO_week_schedule
    FOREIGN KEY (production_id)
    REFERENCES bovelo.production (production_ID);
      