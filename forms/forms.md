# Card-Index activity diagram

## Detailed description of the planning case

### Title : Develop a schedule

**Actor** : Production manager  
**Objective** : Production manager realizes the planning every week based on order  
**Preconditions** : Production manager must have an account  
**Results** : The orders are present in the planning  
**Limit** : Do not exceed the maximum production (to be completed)  
**Flow** :

1) Get orders
2) Add Order to planning
3) Spread the orders over the week

## Detailed description of stock

### Title : Manage the stock

**Actor** : Production Manager and the app  
**Objective** : Plan for minimum stock and optimize stock quantities  
**Preconditions** : Have access to the status of the stock and the orders placed  
**Results** : The stock is updated, the missing parts orders are received  
**Limit** : /  
**Flow** :

1) Check the state of the stock
2) Update the stock with the new parts

## Detailed description of sales

### Title : sell bikes

**Actor** : Representatives  
**Objective** : Be able to show the catalog to the client and estimate the date of delivery and save an order  
**Preconditions** : Account Representative, have access to the planning production  
**Results** : The order are saved  
**Limit** : /  
**Float** :

1) Show the catalog
2) The representative takes the order
3) the order is saved

## Detailed description of the production

### Title : Production of a bike

**Actor** : Mechanic  
**Objective** : Assemble the bike  
**Preconditions** : Have sufficient parts  
**Results** : the bike is assembled  
**Limit** : /  
**Float** :

1) get parts list
2) get parts
3) build bike
