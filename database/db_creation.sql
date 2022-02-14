
                
CREATE TABLE Catalog
(
    ID          INTEGER    NOT NULL,
    model       LINESTRING NOT NULL,
    color       INTEGER    NOT NULL,
    size        INTEGER    NOT NULL,
    description LINESTRING NULL    ,
    PRIMARY KEY (ID)
);

CREATE TABLE Components
(
    part_number INTEGER    NOT NULL,
    ID          INTEGER    NOT NULL,
    amount      INTEGER    NULL    ,
    in_stock    INTEGER    NULL    ,
    description LINESTRING NULL    ,
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
    PRIMARY KEY (invoice_number)
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

                
            