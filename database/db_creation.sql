# ************************************************************
# Sequel Ace SQL dump
# Version 20033
#
# https://sequel-ace.com/
# https://github.com/Sequel-Ace/Sequel-Ace
#
# Host: fredcorp.cc (MySQL 8.0.27)
# Database: bovelo
# Generation Time: 2022-06-04 11:00:57 +0000
# ************************************************************


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
SET NAMES utf8mb4;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE='NO_AUTO_VALUE_ON_ZERO', SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


# Dump of table Catalog
# ------------------------------------------------------------

DROP TABLE IF EXISTS `Catalog`;

CREATE TABLE `Catalog` (
  `ID` int NOT NULL,
  `stock` int DEFAULT NULL,
  `model` varchar(255) NOT NULL,
  `color` int NOT NULL,
  `size` int NOT NULL,
  `price` int DEFAULT NULL,
  `description` varchar(512) DEFAULT NULL,
  `specs` varchar(255) DEFAULT NULL,
  `image` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



# Dump of table Colors
# ------------------------------------------------------------

DROP TABLE IF EXISTS `Colors`;

CREATE TABLE `Colors` (
  `ID` int NOT NULL,
  `description` varchar(255) DEFAULT NULL,
  `hex` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



# Dump of table componentlink
# ------------------------------------------------------------

DROP TABLE IF EXISTS `componentlink`;

CREATE TABLE `componentlink` (
  `part_number` int NOT NULL,
  `ID` int NOT NULL,
  `amount` int DEFAULT NULL,
  KEY `FK_Components_TO_componentlink` (`part_number`),
  KEY `FK_Catalog_TO_componentlink` (`ID`),
  CONSTRAINT `FK_Catalog_TO_componentlink` FOREIGN KEY (`ID`) REFERENCES `Catalog` (`ID`),
  CONSTRAINT `FK_Components_TO_componentlink` FOREIGN KEY (`part_number`) REFERENCES `Components` (`part_number`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



# Dump of table Components
# ------------------------------------------------------------

DROP TABLE IF EXISTS `Components`;

CREATE TABLE `Components` (
  `part_number` int NOT NULL,
  `in_stock` int DEFAULT NULL,
  `minimum_stock` int DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL,
  `location` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`part_number`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



# Dump of table customers
# ------------------------------------------------------------

DROP TABLE IF EXISTS `customers`;

CREATE TABLE `customers` (
  `customer_number` int NOT NULL AUTO_INCREMENT,
  `firstname` varchar(255) DEFAULT NULL,
  `lastname` varchar(255) DEFAULT NULL,
  `address` varchar(255) DEFAULT NULL,
  `phone` varchar(255) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`customer_number`)
) ENGINE=InnoDB AUTO_INCREMENT=122 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



# Dump of table invoice_details
# ------------------------------------------------------------

DROP TABLE IF EXISTS `invoice_details`;

CREATE TABLE `invoice_details` (
  `invoice_number` int NOT NULL,
  `invoice_detail_id` int NOT NULL AUTO_INCREMENT,
  `ID` int NOT NULL,
  `amount` int DEFAULT NULL,
  `price` int DEFAULT NULL,
  `status` varchar(255) NOT NULL DEFAULT 'waiting',
  PRIMARY KEY (`invoice_detail_id`),
  KEY `FK_invoices_TO_invoice_details` (`invoice_number`),
  KEY `FK_Catalog_TO_invoice_details` (`ID`),
  CONSTRAINT `FK_Catalog_TO_invoice_details` FOREIGN KEY (`ID`) REFERENCES `Catalog` (`ID`),
  CONSTRAINT `FK_invoices_TO_invoice_details` FOREIGN KEY (`invoice_number`) REFERENCES `invoices` (`invoice_number`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



# Dump of table invoices
# ------------------------------------------------------------

DROP TABLE IF EXISTS `invoices`;

CREATE TABLE `invoices` (
  `invoice_number` int NOT NULL AUTO_INCREMENT,
  `customer_number` int NOT NULL,
  `date` date DEFAULT NULL,
  `totalPrice` int DEFAULT NULL,
  PRIMARY KEY (`invoice_number`),
  KEY `FK_customers_TO_invoices` (`customer_number`),
  CONSTRAINT `FK_customers_TO_invoices` FOREIGN KEY (`customer_number`) REFERENCES `customers` (`customer_number`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



# Dump of table part_orders
# ------------------------------------------------------------

DROP TABLE IF EXISTS `part_orders`;

CREATE TABLE `part_orders` (
  `order_number` int NOT NULL AUTO_INCREMENT,
  `part_number` int NOT NULL,
  `amount` decimal(10,0) NOT NULL,
  `status` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`order_number`),
  KEY `FK_components_TO_orders` (`part_number`),
  CONSTRAINT `FK_components_TO_orders` FOREIGN KEY (`part_number`) REFERENCES `Components` (`part_number`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



# Dump of table production
# ------------------------------------------------------------

DROP TABLE IF EXISTS `production`;

CREATE TABLE `production` (
  `production_ID` int NOT NULL AUTO_INCREMENT,
  `invoice_detail_id` int NOT NULL,
  `ID` int NOT NULL,
  `amount` int DEFAULT '0',
  `amount_scheduled` int DEFAULT '0',
  `amount_completed` int DEFAULT '0',
  PRIMARY KEY (`production_ID`),
  KEY `FK_Catalog_TO_production` (`ID`),
  KEY `FK_invoice_details_TO_production` (`invoice_detail_id`),
  CONSTRAINT `FK_Catalog_TO_production` FOREIGN KEY (`ID`) REFERENCES `Catalog` (`ID`),
  CONSTRAINT `FK_invoice_details_TO_production` FOREIGN KEY (`invoice_detail_id`) REFERENCES `invoice_details` (`invoice_detail_id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



# Dump of table schedule
# ------------------------------------------------------------

DROP TABLE IF EXISTS `schedule`;

CREATE TABLE `schedule` (
  `bike_id` int NOT NULL AUTO_INCREMENT,
  `ID` int NOT NULL,
  `production_id` int NOT NULL,
  `date` date DEFAULT NULL,
  PRIMARY KEY (`bike_id`),
  KEY `FK_Catalog_week_schedule_ID` (`ID`),
  KEY `FK_production_TO_week_schedule` (`production_id`),
  CONSTRAINT `FK_Catalog_week_schedule_ID` FOREIGN KEY (`ID`) REFERENCES `Catalog` (`ID`),
  CONSTRAINT `FK_production_TO_week_schedule` FOREIGN KEY (`production_id`) REFERENCES `production` (`production_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;




/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
