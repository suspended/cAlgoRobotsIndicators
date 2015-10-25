# MYSQL 

# timezone GMT
SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

# <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
# mysql_query("SET character_set_results = 'utf8', character_set_client = 'utf8', character_set_connection = 'utf8', character_set_database = 'utf8', character_set_server = 'utf8'");
SET NAMES "utf8";
SET NAMES 'utf8' COLLATE 'utf8_general_ci';
SET CHARSET "utf8";
SET CHARACTER SET "utf8";

# create database
CREATE DATABASE db DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_general_ci;
CREATE DATABASE IF NOT EXISTS `db` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;

# use database
USE db;

# Create copier user with ip address and only select privilages
GRANT SELECT ON database.* TO user@'1.2.3.%' IDENTIFIED BY 'password';
#GRANT SELECT, INSERT, DELETE ON database TO username@'localhost' IDENTIFIED BY 'password';

# create user with password
CREATE USER 'user'@'localhost' IDENTIFIED BY 'pass';
GRANT ALL PRIVILEGES ON db.* TO 'user'@'localhost';
GRANT ALL PRIVILEGES ON *.* TO 'user'@'localhost';
# GRANT ALL PRIVILEGES ON *.* TO 'USERNAME'@'1.2.3.4' IDENTIFIED BY 'PASSWORD' WITH GRANT OPTION;
# GRANT ALL PRIVILEGES ON *.* TO 'USERNAME'@'%' IDENTIFIED BY 'PASSWORD' WITH GRANT OPTION;
FLUSH PRIVILEGES;

# [type of permission]
# ALL PRIVILEGES as we saw previously, this would allow a MySQL user all access to a designated database (or if no database is selected, across the system)
# CREATE allows them to create new tables or databases
# DROP allows them to them to delete tables or databases
# DELETE allows them to delete rows from tables
# INSERT allows them to insert rows into tables
# SELECT allows them to use the Select command to read through databases
# UPDATE allow them to update table rows
# GRANT OPTION allows them to grant or remove other users' privileges

# if you need revoke permissions
REVOKE [type of permission] ON [database name].[table name] FROM ‘[username]’@‘localhost’;
REVOKE ALL PRIVILEGES, GRANT OPTION FROM 'USERNAME'@'%';
REVOKE ALL PRIVILEGES, GRANT OPTION FROM 'USERNAME'@'1.2.3.4';

# Delete user
DROP USER 'user'@'localhost';

create table account(id INT NOT NULL AUTO_INCREMENT,time int, accountid int, balance float(10,2),equity float(10,2),margin float(10,2),freemargin float(10,2), currency varchar(20), leverage int, PRIMARY KEY(id));
	

CREATE TABLE IF NOT EXISTS `OpenSignal` (
  `id` varchar(250) DEFAULT NULL,
  `symbol` varchar(250) DEFAULT '0',
  `volume` float DEFAULT '0',
  `type` varchar(250) DEFAULT '0',
  `opent` bigint(21) NOT NULL DEFAULT '0',
  `openp` float(10,6) DEFAULT '0',
  `sl` float(10,6) DEFAULT '0',
  `tp` float(10,6) DEFAULT '0',
  `time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `account` varchar(250) DEFAULT '0',
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE IF NOT EXISTS `CloseSignal` (
  `id` varchar(250) DEFAULT NULL,
  `closet` bigint(21) NOT NULL DEFAULT '0',
  `closep` float(10,6) DEFAULT '0',
  `profit` float(10,6) DEFAULT '0',
  `pips` float(10,2) DEFAULT '0',
  `time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `account` varchar(250) DEFAULT '0',
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE IF NOT EXISTS `GBPJPY` (
  `time` bigint DEFAULT NULL,
  `open` float(10,6) DEFAULT '0',
  `close` float(10,6) DEFAULT '0',
  `low` float(10,6) DEFAULT '0',
  `high` float(10,6) DEFAULT '0',
  `reg` float(10,6) DEFAULT '0',
  `reghigh` float(10,6) DEFAULT '0',  
  `reglow` float(10,6) DEFAULT '0',
  UNIQUE KEY `time` (`time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE IF NOT EXISTS `EURUSD` (
  `time` bigint DEFAULT NULL,
  `open` float(10,6) DEFAULT '0',
  `close` float(10,6) DEFAULT '0',
  `low` float(10,6) DEFAULT '0',
  `high` float(10,6) DEFAULT '0',
  `reg` float(10,6) DEFAULT '0',
  `reghigh` float(10,6) DEFAULT '0',  
  `reglow` float(10,6) DEFAULT '0',
  UNIQUE KEY `time` (`time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


