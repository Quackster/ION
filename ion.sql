/*
MySQL Data Transfer
Source Host: localhost
Source Database: ion
Target Host: localhost
Target Database: ion
Date: 25-1-2009 20:13:28
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for users
-- ----------------------------
CREATE TABLE `users` (
  `id` int(10) unsigned NOT NULL auto_increment,
  `username` varchar(15) collate latin1_general_ci NOT NULL,
  `password` varchar(9) collate latin1_general_ci NOT NULL,
  `signedup` date NOT NULL,
  `email` varchar(50) collate latin1_general_ci NOT NULL,
  `dob` char(10) collate latin1_general_ci NOT NULL,
  `motto` varchar(25) collate latin1_general_ci NOT NULL,
  `figure` char(25) collate latin1_general_ci NOT NULL,
  `gender` varchar(1) collate latin1_general_ci NOT NULL,
  `coins` int(10) unsigned NOT NULL,
  `films` int(10) unsigned NOT NULL,
  `gametickets` int(10) unsigned NOT NULL,
  PRIMARY KEY  (`id`,`username`)
) ENGINE=MyISAM AUTO_INCREMENT=10 DEFAULT CHARSET=latin1 COLLATE=latin1_general_ci;

-- ----------------------------
-- Records 
-- ----------------------------
INSERT INTO `users` VALUES ('9', 'Nillus', 'test123', '2009-01-25', 'meep@meepmail.com', '26.03.1992', 'Billy Jean!', '1150120723280013050122525', 'M', '0', '0', '0');
