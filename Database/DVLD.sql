-- MySQL dump 10.13  Distrib 9.6.0, for macos15 (arm64)
--
-- Host: localhost    Database: DVLD
-- ------------------------------------------------------
-- Server version	9.6.0

CREATE DATABASE IF NOT EXISTS DVLD
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE DVLD;

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Applications`
--

DROP TABLE IF EXISTS `Applications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Applications` (
  `ApplicationID` int NOT NULL AUTO_INCREMENT,
  `ApplicantPersonID` int NOT NULL,
  `ApplicationDate` date NOT NULL,
  `ApplicationTypeID` int NOT NULL,
  `ApplicationStatusID` int NOT NULL,
  `LastStatusDate` date NOT NULL,
  `PaidFees` decimal(10,2) NOT NULL,
  `CreatedByUserID` int NOT NULL,
  PRIMARY KEY (`ApplicationID`),
  KEY `FK_Applications_People` (`ApplicantPersonID`),
  KEY `FK_Applications_ApplicationTypes` (`ApplicationTypeID`),
  KEY `FK_Applications_Users` (`CreatedByUserID`),
  KEY `FK_Applications_ApplicationStatuses` (`ApplicationStatusID`),
  CONSTRAINT `FK_Applications_ApplicationStatuses` FOREIGN KEY (`ApplicationStatusID`) REFERENCES `ApplicationStatuses` (`ApplicationStatusID`),
  CONSTRAINT `FK_Applications_ApplicationTypes` FOREIGN KEY (`ApplicationTypeID`) REFERENCES `ApplicationTypes` (`ApplicationTypeID`),
  CONSTRAINT `FK_Applications_People` FOREIGN KEY (`ApplicantPersonID`) REFERENCES `People` (`PersonID`),
  CONSTRAINT `FK_Applications_Users` FOREIGN KEY (`CreatedByUserID`) REFERENCES `Users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Applications`
--

LOCK TABLES `Applications` WRITE;
/*!40000 ALTER TABLE `Applications` DISABLE KEYS */;
INSERT INTO `Applications` VALUES (1,2,'2026-07-28',1,3,'2026-09-06',15.00,1),(2,2,'2026-08-26',7,3,'2026-08-31',5.00,1),(3,2,'2026-10-03',6,3,'2026-10-06',50.00,1),(4,3,'2010-01-05',1,3,'2010-02-05',15.00,1),(5,4,'2014-06-07',1,3,'2014-07-07',15.00,1),(6,5,'2013-12-12',1,3,'2014-01-12',15.00,1),(7,6,'2020-05-03',1,3,'2020-06-03',15.00,1),(8,7,'2024-07-08',1,3,'2024-08-08',15.00,1),(9,3,'2022-04-23',2,3,'2022-04-25',7.00,1),(10,5,'2017-07-01',3,3,'2017-07-03',10.00,1),(11,6,'2018-02-09',4,3,'2018-02-11',5.00,1),(12,6,'2024-01-03',5,3,'2024-01-04',15.00,1);
/*!40000 ALTER TABLE `Applications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ApplicationStatuses`
--

DROP TABLE IF EXISTS `ApplicationStatuses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ApplicationStatuses` (
  `ApplicationStatusID` int NOT NULL AUTO_INCREMENT,
  `StatusName` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`ApplicationStatusID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ApplicationStatuses`
--

LOCK TABLES `ApplicationStatuses` WRITE;
/*!40000 ALTER TABLE `ApplicationStatuses` DISABLE KEYS */;
INSERT INTO `ApplicationStatuses` VALUES (1,'New'),(2,'Cancelled'),(3,'Completed');
/*!40000 ALTER TABLE `ApplicationStatuses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ApplicationTypes`
--

DROP TABLE IF EXISTS `ApplicationTypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ApplicationTypes` (
  `ApplicationTypeID` int NOT NULL AUTO_INCREMENT,
  `ApplicationTypeTitle` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ApplicationFees` decimal(10,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`ApplicationTypeID`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ApplicationTypes`
--

LOCK TABLES `ApplicationTypes` WRITE;
/*!40000 ALTER TABLE `ApplicationTypes` DISABLE KEYS */;
INSERT INTO `ApplicationTypes` VALUES (1,'New Local Driving License Service',15.00),(2,'Renew Driving License Service',7.00),(3,'Replacement for a Lost Driving License',10.00),(4,'Replacement for a Damaged Driving License',5.00),(5,'Release Detained Driving License',15.00),(6,'New International License',50.00),(7,'Retake Test',5.00);
/*!40000 ALTER TABLE `ApplicationTypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Countries`
--

DROP TABLE IF EXISTS `Countries`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Countries` (
  `CountryID` int NOT NULL AUTO_INCREMENT,
  `CountryName` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`CountryID`)
) ENGINE=InnoDB AUTO_INCREMENT=194 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Countries`
--

LOCK TABLES `Countries` WRITE;
/*!40000 ALTER TABLE `Countries` DISABLE KEYS */;
INSERT INTO `Countries` VALUES (1,'Afghanistan'),(2,'Albania'),(3,'Algeria'),(4,'Andorra'),(5,'Angola'),(6,'Antigua and Barbuda'),(7,'Argentina'),(8,'Armenia'),(9,'Australia'),(10,'Austria'),(11,'Azerbaijan'),(12,'Bahamas'),(13,'Bahrain'),(14,'Bangladesh'),(15,'Barbados'),(16,'Belarus'),(17,'Belgium'),(18,'Belize'),(19,'Benin'),(20,'Bhutan'),(21,'Bolivia'),(22,'Bosnia and Herzegovina'),(23,'Botswana'),(24,'Brazil'),(25,'Brunei'),(26,'Bulgaria'),(27,'Burkina Faso'),(28,'Burundi'),(29,'Cabo Verde'),(30,'Cambodia'),(31,'Cameroon'),(32,'Canada'),(33,'Central African Republic'),(34,'Chad'),(35,'Chile'),(36,'China'),(37,'Colombia'),(38,'Comoros'),(39,'Costa Rica'),(40,'Croatia'),(41,'Cuba'),(42,'Cyprus'),(43,'Czech Republic'),(44,'Democratic Republic of the Congo'),(45,'Denmark'),(46,'Djibouti'),(47,'Dominica'),(48,'Dominican Republic'),(49,'Ecuador'),(50,'Egypt'),(51,'El Salvador'),(52,'Equatorial Guinea'),(53,'Eritrea'),(54,'Estonia'),(55,'Eswatini'),(56,'Ethiopia'),(57,'Fiji'),(58,'Finland'),(59,'France'),(60,'Gabon'),(61,'Gambia'),(62,'Georgia'),(63,'Germany'),(64,'Ghana'),(65,'Greece'),(66,'Grenada'),(67,'Guatemala'),(68,'Guinea'),(69,'Guinea-Bissau'),(70,'Guyana'),(71,'Haiti'),(72,'Honduras'),(73,'Hungary'),(74,'Iceland'),(75,'India'),(76,'Indonesia'),(77,'Iran'),(78,'Iraq'),(79,'Ireland'),(80,'Israel'),(81,'Italy'),(82,'Jamaica'),(83,'Japan'),(84,'Jordan'),(85,'Kazakhstan'),(86,'Kenya'),(87,'Kiribati'),(88,'Kuwait'),(89,'Kyrgyzstan'),(90,'Laos'),(91,'Latvia'),(92,'Lebanon'),(93,'Lesotho'),(94,'Liberia'),(95,'Libya'),(96,'Liechtenstein'),(97,'Lithuania'),(98,'Luxembourg'),(99,'Madagascar'),(100,'Malawi'),(101,'Malaysia'),(102,'Maldives'),(103,'Mali'),(104,'Malta'),(105,'Marshall Islands'),(106,'Mauritania'),(107,'Mauritius'),(108,'Mexico'),(109,'Micronesia'),(110,'Moldova'),(111,'Monaco'),(112,'Mongolia'),(113,'Montenegro'),(114,'Morocco'),(115,'Mozambique'),(116,'Myanmar'),(117,'Namibia'),(118,'Nauru'),(119,'Nepal'),(120,'Netherlands'),(121,'New Zealand'),(122,'Nicaragua'),(123,'Niger'),(124,'Nigeria'),(125,'North Korea'),(126,'North Macedonia'),(127,'Norway'),(128,'Oman'),(129,'Pakistan'),(130,'Palau'),(131,'Panama'),(132,'Papua New Guinea'),(133,'Paraguay'),(134,'Peru'),(135,'Philippines'),(136,'Poland'),(137,'Portugal'),(138,'Qatar'),(139,'Romania'),(140,'Russia'),(141,'Rwanda'),(142,'Saint Kitts and Nevis'),(143,'Saint Lucia'),(144,'Saint Vincent and the Grenadines'),(145,'Samoa'),(146,'San Marino'),(147,'Sao Tome and Principe'),(148,'Saudi Arabia'),(149,'Senegal'),(150,'Serbia'),(151,'Seychelles'),(152,'Sierra Leone'),(153,'Singapore'),(154,'Slovakia'),(155,'Slovenia'),(156,'Solomon Islands'),(157,'Somalia'),(158,'South Africa'),(159,'South Korea'),(160,'South Sudan'),(161,'Spain'),(162,'Sri Lanka'),(163,'Sudan'),(164,'Suriname'),(165,'Sweden'),(166,'Switzerland'),(167,'Syria'),(168,'Taiwan'),(169,'Tajikistan'),(170,'Tanzania'),(171,'Thailand'),(172,'Timor-Leste'),(173,'Togo'),(174,'Tonga'),(175,'Trinidad and Tobago'),(176,'Tunisia'),(177,'Turkey'),(178,'Turkmenistan'),(179,'Tuvalu'),(180,'Uganda'),(181,'Ukraine'),(182,'United Arab Emirates'),(183,'United Kingdom'),(184,'United States'),(185,'Uruguay'),(186,'Uzbekistan'),(187,'Vanuatu'),(188,'Vatican City'),(189,'Venezuela'),(190,'Vietnam'),(191,'Yemen'),(192,'Zambia'),(193,'Zimbabwe');
/*!40000 ALTER TABLE `Countries` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `DetainedLicenses`
--

DROP TABLE IF EXISTS `DetainedLicenses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DetainedLicenses` (
  `DetainID` int NOT NULL AUTO_INCREMENT,
  `LicenseID` int NOT NULL,
  `DetainDate` date NOT NULL,
  `FineFees` decimal(10,2) NOT NULL,
  `CreatedByUserID` int NOT NULL,
  `ReleaseDate` date DEFAULT NULL,
  `ReleasedByUserID` int DEFAULT NULL,
  `ReleaseApplicationID` int DEFAULT NULL,
  PRIMARY KEY (`DetainID`),
  UNIQUE KEY `UQ_DetainedLicenses_ReleaseApplicationID` (`ReleaseApplicationID`),
  KEY `FK_DetainedLicenses_Licenses` (`LicenseID`),
  KEY `FK_DetainedLicenses_CreatingUser` (`CreatedByUserID`),
  KEY `FK_DetainedLicenses_ReleasingUser` (`ReleasedByUserID`),
  CONSTRAINT `FK_DetainedLicenses_Applications` FOREIGN KEY (`ReleaseApplicationID`) REFERENCES `Applications` (`ApplicationID`),
  CONSTRAINT `FK_DetainedLicenses_CreatingUser` FOREIGN KEY (`CreatedByUserID`) REFERENCES `Users` (`UserID`),
  CONSTRAINT `FK_DetainedLicenses_Licenses` FOREIGN KEY (`LicenseID`) REFERENCES `Licenses` (`LicenseID`),
  CONSTRAINT `FK_DetainedLicenses_ReleasingUser` FOREIGN KEY (`ReleasedByUserID`) REFERENCES `Users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `DetainedLicenses`
--

LOCK TABLES `DetainedLicenses` WRITE;
/*!40000 ALTER TABLE `DetainedLicenses` DISABLE KEYS */;
INSERT INTO `DetainedLicenses` VALUES (1,5,'2023-12-31',100.00,1,'2024-04-01',1,12);
/*!40000 ALTER TABLE `DetainedLicenses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Drivers`
--

DROP TABLE IF EXISTS `Drivers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Drivers` (
  `DriverID` int NOT NULL AUTO_INCREMENT,
  `PersonID` int NOT NULL,
  `CreatedByUserID` int NOT NULL,
  `CreatedDate` date NOT NULL,
  PRIMARY KEY (`DriverID`),
  UNIQUE KEY `UQ_Drivers_PersonID` (`PersonID`),
  KEY `FK_Drivers_Users` (`CreatedByUserID`),
  CONSTRAINT `FK_Drivers_People` FOREIGN KEY (`PersonID`) REFERENCES `People` (`PersonID`),
  CONSTRAINT `FK_Drivers_Users` FOREIGN KEY (`CreatedByUserID`) REFERENCES `Users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Drivers`
--

LOCK TABLES `Drivers` WRITE;
/*!40000 ALTER TABLE `Drivers` DISABLE KEYS */;
INSERT INTO `Drivers` VALUES (1,2,1,'2026-09-06'),(2,3,1,'2010-02-05'),(3,4,1,'2014-07-07'),(4,5,1,'2014-01-12'),(5,6,1,'2020-06-03'),(6,7,1,'2024-08-08');
/*!40000 ALTER TABLE `Drivers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Genders`
--

DROP TABLE IF EXISTS `Genders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Genders` (
  `GenderID` int NOT NULL AUTO_INCREMENT,
  `GenderName` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`GenderID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Genders`
--

LOCK TABLES `Genders` WRITE;
/*!40000 ALTER TABLE `Genders` DISABLE KEYS */;
INSERT INTO `Genders` VALUES (1,'Male'),(2,'Female');
/*!40000 ALTER TABLE `Genders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `InternationalLicenses`
--

DROP TABLE IF EXISTS `InternationalLicenses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `InternationalLicenses` (
  `InternationalLicenseID` int NOT NULL AUTO_INCREMENT,
  `ApplicationID` int NOT NULL,
  `DriverID` int NOT NULL,
  `IssuedUsingLocalLicenseID` int NOT NULL,
  `IssueDate` date NOT NULL,
  `ExpirationDate` date NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedByUserID` int NOT NULL,
  PRIMARY KEY (`InternationalLicenseID`),
  UNIQUE KEY `UQ_InternationalLicenses_ApplicationID` (`ApplicationID`),
  KEY `FK_InternationalLicenses_Drivers` (`DriverID`),
  KEY `FK_InternationalLicenses_Licenses` (`IssuedUsingLocalLicenseID`),
  KEY `FK_InternationalLicenses_Users` (`CreatedByUserID`),
  CONSTRAINT `FK_InternationalLicenses_Applications` FOREIGN KEY (`ApplicationID`) REFERENCES `Applications` (`ApplicationID`),
  CONSTRAINT `FK_InternationalLicenses_Drivers` FOREIGN KEY (`DriverID`) REFERENCES `Drivers` (`DriverID`),
  CONSTRAINT `FK_InternationalLicenses_Licenses` FOREIGN KEY (`IssuedUsingLocalLicenseID`) REFERENCES `Licenses` (`LicenseID`),
  CONSTRAINT `FK_InternationalLicenses_Users` FOREIGN KEY (`CreatedByUserID`) REFERENCES `Users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `InternationalLicenses`
--

LOCK TABLES `InternationalLicenses` WRITE;
/*!40000 ALTER TABLE `InternationalLicenses` DISABLE KEYS */;
INSERT INTO `InternationalLicenses` VALUES (1,3,1,1,'2026-10-06','2027-10-06',1,1);
/*!40000 ALTER TABLE `InternationalLicenses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LicenseClasses`
--

DROP TABLE IF EXISTS `LicenseClasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LicenseClasses` (
  `LicenseClassID` int NOT NULL AUTO_INCREMENT,
  `ClassName` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ClassDescription` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL,
  `MinimumAllowedAge` tinyint unsigned NOT NULL,
  `DefaultValidityLength` tinyint unsigned NOT NULL,
  `ClassFees` decimal(10,2) NOT NULL,
  PRIMARY KEY (`LicenseClassID`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LicenseClasses`
--

LOCK TABLES `LicenseClasses` WRITE;
/*!40000 ALTER TABLE `LicenseClasses` DISABLE KEYS */;
INSERT INTO `LicenseClasses` VALUES (1,'Class 1 - Small Motorcycle','It allows the driver to drive small motorcycles, It is suitable for motorcycles with small capacity and limited power.',18,5,15.00),(2,'Class 2 - Heavy Motorcycle License','Heavy Motorcycle License (Large Motorcycle License)',21,5,30.00),(3,'Class 3 - Ordinary driving license','Ordinary driving license (car licence)',18,10,20.00),(4,'Class 4 - Commercial','Commercial driving license (taxi/limousine)',21,10,200.00),(5,'Class 5 - Agricultural','Agricultural and work vehicles used in farming or construction, (tractors / tillage machinery)',21,10,50.00),(6,'Class 6 - Small and medium bus','Small and medium bus license',21,10,250.00),(7,'Class 7 - Truck and heavy vehicle','Truck and heavy vehicle license',21,10,300.00);
/*!40000 ALTER TABLE `LicenseClasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LicenseIssueReasons`
--

DROP TABLE IF EXISTS `LicenseIssueReasons`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LicenseIssueReasons` (
  `IssueReasonID` int NOT NULL AUTO_INCREMENT,
  `IssueReasonName` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`IssueReasonID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LicenseIssueReasons`
--

LOCK TABLES `LicenseIssueReasons` WRITE;
/*!40000 ALTER TABLE `LicenseIssueReasons` DISABLE KEYS */;
INSERT INTO `LicenseIssueReasons` VALUES (1,'First Time Issue'),(2,'Renewal'),(3,'Replacement - Lost'),(4,'Replacement - Damaged');
/*!40000 ALTER TABLE `LicenseIssueReasons` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Licenses`
--

DROP TABLE IF EXISTS `Licenses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Licenses` (
  `LicenseID` int NOT NULL AUTO_INCREMENT,
  `ApplicationID` int NOT NULL,
  `DriverID` int NOT NULL,
  `LicenseClassID` int NOT NULL,
  `IssueDate` date NOT NULL,
  `ExpirationDate` date NOT NULL,
  `Notes` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PaidFees` decimal(10,2) NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `IssueReasonID` int NOT NULL,
  `CreatedByUserID` int NOT NULL,
  PRIMARY KEY (`LicenseID`),
  UNIQUE KEY `UQ_Licenses_ApplicationID` (`ApplicationID`),
  KEY `FK_Licenses_Users` (`CreatedByUserID`),
  KEY `FK_Licenses_LicenseClasses` (`LicenseClassID`),
  KEY `FK_Licenses_Drivers` (`DriverID`),
  KEY `FK_Licenses_LicenseIssueReasons` (`IssueReasonID`),
  CONSTRAINT `FK_Licenses_Applications` FOREIGN KEY (`ApplicationID`) REFERENCES `Applications` (`ApplicationID`),
  CONSTRAINT `FK_Licenses_Drivers` FOREIGN KEY (`DriverID`) REFERENCES `Drivers` (`DriverID`),
  CONSTRAINT `FK_Licenses_LicenseClasses` FOREIGN KEY (`LicenseClassID`) REFERENCES `LicenseClasses` (`LicenseClassID`),
  CONSTRAINT `FK_Licenses_LicenseIssueReasons` FOREIGN KEY (`IssueReasonID`) REFERENCES `LicenseIssueReasons` (`IssueReasonID`),
  CONSTRAINT `FK_Licenses_Users` FOREIGN KEY (`CreatedByUserID`) REFERENCES `Users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Licenses`
--

LOCK TABLES `Licenses` WRITE;
/*!40000 ALTER TABLE `Licenses` DISABLE KEYS */;
INSERT INTO `Licenses` VALUES (1,1,1,3,'2026-09-06','2036-09-06',NULL,20.00,1,1,1),(2,4,2,1,'2010-02-05','2015-02-05',NULL,15.00,0,1,1),(3,5,3,2,'2014-07-07','2019-07-07',NULL,30.00,0,1,1),(4,6,4,4,'2014-01-12','2024-01-12',NULL,200.00,0,1,1),(5,7,5,5,'2020-06-03','2030-06-03',NULL,50.00,1,1,1),(6,8,6,6,'2024-08-08','2034-08-08',NULL,250.00,1,1,1),(7,9,2,1,'2022-04-25','2027-04-25',NULL,15.00,1,2,1),(8,10,3,2,'2014-07-07','2019-07-07',NULL,30.00,0,3,1),(9,11,4,4,'2014-01-12','2024-01-12',NULL,200.00,0,4,1);
/*!40000 ALTER TABLE `Licenses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LocalDrivingLicenseApplications`
--

DROP TABLE IF EXISTS `LocalDrivingLicenseApplications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LocalDrivingLicenseApplications` (
  `LocalDrivingLicenseApplicationID` int NOT NULL AUTO_INCREMENT,
  `ApplicationID` int NOT NULL,
  `LicenseClassID` int NOT NULL,
  PRIMARY KEY (`LocalDrivingLicenseApplicationID`),
  UNIQUE KEY `UQ_LocalDrivingLicenseApplications_ApplicationID` (`ApplicationID`),
  KEY `FK_LocalDrivingLicenseApplications_LicenseClasses` (`LicenseClassID`),
  CONSTRAINT `FK_LocalDrivingLicenseApplications_Applications` FOREIGN KEY (`ApplicationID`) REFERENCES `Applications` (`ApplicationID`),
  CONSTRAINT `FK_LocalDrivingLicenseApplications_LicenseClasses` FOREIGN KEY (`LicenseClassID`) REFERENCES `LicenseClasses` (`LicenseClassID`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LocalDrivingLicenseApplications`
--

LOCK TABLES `LocalDrivingLicenseApplications` WRITE;
/*!40000 ALTER TABLE `LocalDrivingLicenseApplications` DISABLE KEYS */;
INSERT INTO `LocalDrivingLicenseApplications` VALUES (1,1,3),(2,4,1),(3,5,2),(4,6,4),(5,7,5),(6,8,6);
/*!40000 ALTER TABLE `LocalDrivingLicenseApplications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `People`
--

DROP TABLE IF EXISTS `People`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `People` (
  `PersonID` int NOT NULL AUTO_INCREMENT,
  `NationalNo` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `FirstName` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SecondName` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ThirdName` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LastName` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `DateOfBirth` date NOT NULL,
  `GenderID` int NOT NULL,
  `Address` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Phone` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Email` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NationalityCountryID` int NOT NULL,
  `ImagePath` varchar(250) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`PersonID`),
  UNIQUE KEY `UQ_People_NationalNo` (`NationalNo`),
  KEY `FK_People_Countries` (`NationalityCountryID`),
  KEY `FK_People_Genders` (`GenderID`),
  CONSTRAINT `FK_People_Countries` FOREIGN KEY (`NationalityCountryID`) REFERENCES `Countries` (`CountryID`),
  CONSTRAINT `FK_People_Genders` FOREIGN KEY (`GenderID`) REFERENCES `Genders` (`GenderID`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `People`
--

LOCK TABLES `People` WRITE;
/*!40000 ALTER TABLE `People` DISABLE KEYS */;
INSERT INTO `People` VALUES (1,'1000000001','Siraj','Shaker','Jaralla','Alkshali','1997-01-13',1,'Amman, Jordan','0790000001','siraj.shaker@example.com',84,NULL),(2,'1000000002','Yazan','Shaker','Jaralla','Alkshali','1999-05-26',1,'Amman, Jordan','0790000002','yazan.shaker@example.com',84,NULL),(3,'1000000003','Ahmad','Mohammad','Ali','Al-Hassan','1995-03-15',1,'Amman, Jordan','0799000001','ahmad.hassan@example.com',84,NULL),(4,'1000000004','Sara','Khaled',NULL,'Al-Salem','1998-07-22',2,'Irbid, Jordan','0799000002','sara.salem@example.com',84,NULL),(5,'1000000005','Omar','Yousef','Mahmoud','Al-Rashid','1992-11-05',1,'Zarqa, Jordan','0799000003','omar.rashid@example.com',84,NULL),(6,'1000000006','Fatima','Ali','Hassan','Al-Khatib','2000-01-30',2,'Aqaba, Jordan','0799000004','fatima.khatib@example.com',84,NULL),(7,'1000000007','Ali','Hussein',NULL,'Al-Masri','1990-09-18',1,'Amman, Jordan','0799000005','ali.masri@example.com',84,NULL);
/*!40000 ALTER TABLE `People` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TestAppointments`
--

DROP TABLE IF EXISTS `TestAppointments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TestAppointments` (
  `TestAppointmentID` int NOT NULL AUTO_INCREMENT,
  `TestTypeID` int NOT NULL,
  `LocalDrivingLicenseApplicationID` int NOT NULL,
  `AppointmentTime` datetime NOT NULL,
  `PaidFees` decimal(10,2) NOT NULL,
  `CreatedByUserID` int NOT NULL,
  `IsLocked` tinyint(1) NOT NULL DEFAULT '0',
  `RetakeTestApplicationID` int DEFAULT NULL,
  PRIMARY KEY (`TestAppointmentID`),
  UNIQUE KEY `UQ_TestAppointments_Applications` (`RetakeTestApplicationID`),
  KEY `FK_TestAppointments_TestTypes` (`TestTypeID`),
  KEY `FK_TestAppointments_LocalDrivingLicenseApplications` (`LocalDrivingLicenseApplicationID`),
  KEY `FK_TestAppointments_Users` (`CreatedByUserID`),
  CONSTRAINT `FK_TestAppointments_Applications` FOREIGN KEY (`RetakeTestApplicationID`) REFERENCES `Applications` (`ApplicationID`),
  CONSTRAINT `FK_TestAppointments_LocalDrivingLicenseApplications` FOREIGN KEY (`LocalDrivingLicenseApplicationID`) REFERENCES `LocalDrivingLicenseApplications` (`LocalDrivingLicenseApplicationID`),
  CONSTRAINT `FK_TestAppointments_TestTypes` FOREIGN KEY (`TestTypeID`) REFERENCES `TestTypes` (`TestTypeID`),
  CONSTRAINT `FK_TestAppointments_Users` FOREIGN KEY (`CreatedByUserID`) REFERENCES `Users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TestAppointments`
--

LOCK TABLES `TestAppointments` WRITE;
/*!40000 ALTER TABLE `TestAppointments` DISABLE KEYS */;
INSERT INTO `TestAppointments` VALUES (1,1,1,'2026-08-25 08:00:00',10.00,1,1,NULL),(2,1,1,'2026-08-31 08:00:00',10.00,1,1,2),(3,2,1,'2026-09-03 09:00:00',20.00,1,1,NULL),(4,3,1,'2026-09-04 10:00:00',35.00,1,1,NULL),(5,3,1,'2026-09-04 10:00:00',35.00,1,0,NULL),(6,1,2,'2010-01-15 10:00:00',10.00,1,1,NULL),(7,2,2,'2010-01-20 10:00:00',20.00,1,1,NULL),(8,3,2,'2010-02-05 10:00:00',35.00,1,1,NULL),(9,1,3,'2014-06-12 10:00:00',10.00,1,1,NULL),(10,2,3,'2014-06-16 10:00:00',20.00,1,1,NULL),(11,3,3,'2014-07-07 10:00:00',35.00,1,1,NULL),(12,1,4,'2013-12-17 10:00:00',10.00,1,1,NULL),(13,2,4,'2013-12-22 10:00:00',20.00,1,1,NULL),(14,3,4,'2014-01-12 10:00:00',35.00,1,1,NULL),(15,1,5,'2020-05-08 10:00:00',10.00,1,1,NULL),(16,2,5,'2020-05-13 10:00:00',20.00,1,1,NULL),(17,3,5,'2020-06-03 10:00:00',35.00,1,1,NULL),(18,1,6,'2024-07-13 10:00:00',10.00,1,1,NULL),(19,2,6,'2024-07-17 10:00:00',20.00,1,1,NULL),(20,3,6,'2024-08-08 10:00:00',35.00,1,1,NULL);
/*!40000 ALTER TABLE `TestAppointments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Tests`
--

DROP TABLE IF EXISTS `Tests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Tests` (
  `TestID` int NOT NULL AUTO_INCREMENT,
  `TestAppointmentID` int NOT NULL,
  `Passed` tinyint(1) NOT NULL,
  `Notes` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedByUserID` int NOT NULL,
  PRIMARY KEY (`TestID`),
  UNIQUE KEY `UQ_Tests_TestAppointments` (`TestAppointmentID`),
  KEY `FK_Tests_Users` (`CreatedByUserID`),
  CONSTRAINT `FK_Tests_TestAppointments` FOREIGN KEY (`TestAppointmentID`) REFERENCES `TestAppointments` (`TestAppointmentID`),
  CONSTRAINT `FK_Tests_Users` FOREIGN KEY (`CreatedByUserID`) REFERENCES `Users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Tests`
--

LOCK TABLES `Tests` WRITE;
/*!40000 ALTER TABLE `Tests` DISABLE KEYS */;
INSERT INTO `Tests` VALUES (1,1,0,NULL,1),(2,2,1,NULL,1),(3,3,1,NULL,1),(4,4,1,NULL,1),(6,5,1,NULL,1),(7,6,1,NULL,1),(8,7,1,NULL,1),(9,8,1,NULL,1),(10,9,1,NULL,1),(11,10,1,NULL,1),(12,11,1,NULL,1),(13,12,1,NULL,1),(14,13,1,NULL,1),(15,14,1,NULL,1),(16,15,1,NULL,1),(17,16,1,NULL,1),(18,17,1,NULL,1),(19,18,1,NULL,1),(20,19,1,NULL,1);
/*!40000 ALTER TABLE `Tests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TestTypes`
--

DROP TABLE IF EXISTS `TestTypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TestTypes` (
  `TestTypeID` int NOT NULL AUTO_INCREMENT,
  `TestTypeTitle` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TestTypeDescription` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TestTypeFees` decimal(10,2) NOT NULL,
  PRIMARY KEY (`TestTypeID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TestTypes`
--

LOCK TABLES `TestTypes` WRITE;
/*!40000 ALTER TABLE `TestTypes` DISABLE KEYS */;
INSERT INTO `TestTypes` VALUES (1,'Vision Test','This assesses the applicant\'s visual acuity to ensure they have sufficient vision to drive safely.',10.00),(2,'Written (Theory) Test','This test assesses the applicant\'s knowledge of traffic rules, road signs, and driving regulations. It typically consists of multiple-choice questions, and the applicant must select the correct answer(s). The written test aims to ensure that the applicant understands the rules of the road and can apply them in various driving scenarios.',20.00),(3,'Practical (Street) Test','This test evaluates the applicant\'s driving skills and ability to operate a motor vehicle safely on public roads. A licensed examiner accompanies the applicant in the vehicle and observes their driving performance.',35.00);
/*!40000 ALTER TABLE `TestTypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Users`
--

DROP TABLE IF EXISTS `Users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Users` (
  `UserID` int NOT NULL AUTO_INCREMENT,
  `PersonID` int NOT NULL,
  `UserName` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `PasswordHash` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`UserID`),
  UNIQUE KEY `UQ_Users_PersonID` (`PersonID`),
  UNIQUE KEY `UQ_Users_UserName` (`UserName`),
  CONSTRAINT `FK_Users_People` FOREIGN KEY (`PersonID`) REFERENCES `People` (`PersonID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Users`
--

LOCK TABLES `Users` WRITE;
/*!40000 ALTER TABLE `Users` DISABLE KEYS */;
INSERT INTO `Users` VALUES (1,1,'siraj.admin','TEST_HASH_1_NOT_REAL',1);
/*!40000 ALTER TABLE `Users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-07-28  3:01:26
