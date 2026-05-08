-- MySQL dump 10.13  Distrib 9.6.0, for Linux (x86_64)
--
-- Host: localhost    Database: authapp
-- ------------------------------------------------------
-- Server version	9.6.0

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
SET @MYSQLDUMP_TEMP_LOG_BIN = @@SESSION.SQL_LOG_BIN;
SET @@SESSION.SQL_LOG_BIN= 0;

--
-- GTID state at the beginning of the backup 
--

SET @@GLOBAL.GTID_PURGED=/*!80000 '+'*/ 'cd34463b-387b-11f1-b0c5-f64dc28812da:1-1514';

--
-- Table structure for table `Counterparties`
--

DROP TABLE IF EXISTS `Counterparties`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Counterparties` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ShortName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `OtherPhone` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MainPhone` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Website` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MainEmail` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Fax` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatedAt` datetime(6) DEFAULT NULL,
  `Type` int DEFAULT NULL,
  `Industry` int DEFAULT NULL,
  `OrganizationType` int DEFAULT NULL,
  `IinBin` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DoNotSendEmail` tinyint(1) NOT NULL,
  `FacebookUrl` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `VkontakteUrl` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TwitterUrl` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `LastSmsDate` datetime(6) DEFAULT NULL,
  `LegalAddress` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ActualAddress` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Country` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Region` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `City` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ActualCountry` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ActualRegion` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ActualCity` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Counterparties`
--

LOCK TABLES `Counterparties` WRITE;
/*!40000 ALTER TABLE `Counterparties` DISABLE KEYS */;
/*!40000 ALTER TABLE `Counterparties` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `FavoriteDocuments`
--

DROP TABLE IF EXISTS `FavoriteDocuments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `FavoriteDocuments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserEmail` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FileDocumentsId` int NOT NULL,
  `AddedAt` datetime(6) NOT NULL,
  `FavoriteDocumentId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_FavoriteDocuments_FavoriteDocumentId` (`FavoriteDocumentId`),
  KEY `IX_FavoriteDocuments_FileDocumentsId` (`FileDocumentsId`),
  CONSTRAINT `FK_FavoriteDocuments_FavoriteDocuments_FavoriteDocumentId` FOREIGN KEY (`FavoriteDocumentId`) REFERENCES `FavoriteDocuments` (`Id`),
  CONSTRAINT `FK_FavoriteDocuments_FileDocuments_FileDocumentsId` FOREIGN KEY (`FileDocumentsId`) REFERENCES `FileDocuments` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `FavoriteDocuments`
--

LOCK TABLES `FavoriteDocuments` WRITE;
/*!40000 ALTER TABLE `FavoriteDocuments` DISABLE KEYS */;
/*!40000 ALTER TABLE `FavoriteDocuments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `FileDocuments`
--

DROP TABLE IF EXISTS `FileDocuments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `FileDocuments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FileName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `FilePath` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Extension` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UploadDate` datetime(6) NOT NULL,
  `DeletedAt` datetime(6) DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `FileDocuments`
--

LOCK TABLES `FileDocuments` WRITE;
/*!40000 ALTER TABLE `FileDocuments` DISABLE KEYS */;
/*!40000 ALTER TABLE `FileDocuments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `FileTags`
--

DROP TABLE IF EXISTS `FileTags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `FileTags` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FileDocumentsId` int NOT NULL,
  `TagId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_FileTags_FileDocumentsId` (`FileDocumentsId`),
  KEY `IX_FileTags_TagId` (`TagId`),
  CONSTRAINT `FK_FileTags_FileDocuments_FileDocumentsId` FOREIGN KEY (`FileDocumentsId`) REFERENCES `FileDocuments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_FileTags_Tags_TagId` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `FileTags`
--

LOCK TABLES `FileTags` WRITE;
/*!40000 ALTER TABLE `FileTags` DISABLE KEYS */;
/*!40000 ALTER TABLE `FileTags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `KatoEntries`
--

DROP TABLE IF EXISTS `KatoEntries`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `KatoEntries` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Ab` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Cd` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Ef` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Hij` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Level` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `KazName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RusName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `KatoEntries`
--

LOCK TABLES `KatoEntries` WRITE;
/*!40000 ALTER TABLE `KatoEntries` DISABLE KEYS */;
/*!40000 ALTER TABLE `KatoEntries` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LoginHistories`
--

DROP TABLE IF EXISTS `LoginHistories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LoginHistories` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `LoginTime` datetime(6) NOT NULL,
  `IPAddress` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LoginHistories`
--

LOCK TABLES `LoginHistories` WRITE;
/*!40000 ALTER TABLE `LoginHistories` DISABLE KEYS */;
INSERT INTO `LoginHistories` VALUES (1,'elita700@gmail.com','2026-05-08 09:10:06.234222','::1');
/*!40000 ALTER TABLE `LoginHistories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Roles`
--

DROP TABLE IF EXISTS `Roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Roles` (
  `RoleId` int NOT NULL AUTO_INCREMENT,
  `RoleName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`RoleId`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Roles`
--

LOCK TABLES `Roles` WRITE;
/*!40000 ALTER TABLE `Roles` DISABLE KEYS */;
INSERT INTO `Roles` VALUES (1,'User'),(2,'Admin');
/*!40000 ALTER TABLE `Roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TagCategories`
--

DROP TABLE IF EXISTS `TagCategories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TagCategories` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TagCategories`
--

LOCK TABLES `TagCategories` WRITE;
/*!40000 ALTER TABLE `TagCategories` DISABLE KEYS */;
/*!40000 ALTER TABLE `TagCategories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TagCategoryTags`
--

DROP TABLE IF EXISTS `TagCategoryTags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TagCategoryTags` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `TagId` int NOT NULL,
  `TagCategoryId` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_TagCategoryTags_TagId_TagCategoryId` (`TagId`,`TagCategoryId`),
  KEY `IX_TagCategoryTags_TagCategoryId` (`TagCategoryId`),
  CONSTRAINT `FK_TagCategoryTags_TagCategories_TagCategoryId` FOREIGN KEY (`TagCategoryId`) REFERENCES `TagCategories` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_TagCategoryTags_Tags_TagId` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TagCategoryTags`
--

LOCK TABLES `TagCategoryTags` WRITE;
/*!40000 ALTER TABLE `TagCategoryTags` DISABLE KEYS */;
/*!40000 ALTER TABLE `TagCategoryTags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Tags`
--

DROP TABLE IF EXISTS `Tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Tags` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Tags`
--

LOCK TABLES `Tags` WRITE;
/*!40000 ALTER TABLE `Tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `Tags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `UserActionLog`
--

DROP TABLE IF EXISTS `UserActionLog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `UserActionLog` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserEmail` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Action` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ActionTime` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `UserActionLog`
--

LOCK TABLES `UserActionLog` WRITE;
/*!40000 ALTER TABLE `UserActionLog` DISABLE KEYS */;
INSERT INTO `UserActionLog` VALUES (1,'elita700@gmail.com','Зарегистрировался','2026-05-08 09:06:57.220849'),(2,'elita700@gmail.com','Вход в аккаунт','2026-05-08 09:10:06.399440');
/*!40000 ALTER TABLE `UserActionLog` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Users`
--

DROP TABLE IF EXISTS `Users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Users` (
  `UserId` int NOT NULL AUTO_INCREMENT,
  `FullName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Login` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `RoleId` int NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ConfirmationToken` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ConfirmationCode` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IsConfirmed` tinyint(1) NOT NULL,
  `CreateAt` datetime(6) NOT NULL,
  `PasswordResetCode` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PasswordResetExpiresAt` datetime(6) DEFAULT NULL,
  `LoginCount` int NOT NULL,
  `IsBlocked` tinyint(1) NOT NULL DEFAULT '0',
  `District` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `House` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Latitude` double DEFAULT NULL,
  `Longitude` double DEFAULT NULL,
  `RuralDistrict` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Settlement` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Street` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`UserId`),
  KEY `IX_Users_RoleId` (`RoleId`),
  CONSTRAINT `FK_Users_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`RoleId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Users`
--

LOCK TABLES `Users` WRITE;
/*!40000 ALTER TABLE `Users` DISABLE KEYS */;
INSERT INTO `Users` VALUES (1,'Митя','elita700@gmail.com','elita700',2,'2OAtw4n0dDIL9sqT+a4A/OYuMZpSsp594EywZY3aFo0=','fad3e7b7-ed35-4725-836c-822688d1c8e0',NULL,1,'2026-05-08 09:06:22.138662',NULL,NULL,1,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `Users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `__EFMigrationsHistory`
--

DROP TABLE IF EXISTS `__EFMigrationsHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__EFMigrationsHistory`
--

LOCK TABLES `__EFMigrationsHistory` WRITE;
/*!40000 ALTER TABLE `__EFMigrationsHistory` DISABLE KEYS */;
INSERT INTO `__EFMigrationsHistory` VALUES ('20260424031913_InitClean','9.0.0'),('20260424040144_AddTags','9.0.0'),('20260427042934_AddIsBlockedToUser','9.0.0'),('20260428033339_TagCategoryManyToMany','9.0.0'),('20260428034729_TagCategoryManyToMany2','9.0.0'),('20260504044534_AddTrashToFileDocuments','9.0.0'),('20260504055525_AddFavoriteDocuments','9.0.0'),('20260504072331_AddLocationFields','9.0.0'),('20260504092844_AddKatoTable','9.0.0'),('20260506040817_AddCounterparty','9.0.0'),('20260508024335_FixMissingTables','9.0.0');
/*!40000 ALTER TABLE `__EFMigrationsHistory` ENABLE KEYS */;
UNLOCK TABLES;
SET @@SESSION.SQL_LOG_BIN = @MYSQLDUMP_TEMP_LOG_BIN;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-05-08  3:21:26
