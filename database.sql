/*
Navicat PGSQL Data Transfer

Source Server         : London
Source Server Version : 90113
Source Host           : localhost:5432
Source Database       : webscrape
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90113
File Encoding         : 65001

Date: 2014-04-28 21:01:26
*/


-- ----------------------------
-- Table structure for headers
-- ----------------------------
DROP TABLE IF EXISTS "public"."headers";
CREATE TABLE "public"."headers" (
"headerid" int4 NOT NULL,
"siteid" int4,
"header_name" text COLLATE "default",
"header_value" text COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for sites
-- ----------------------------
DROP TABLE IF EXISTS "public"."sites";
CREATE TABLE "public"."sites" (
"site_id" int4 NOT NULL,
"site_url" text COLLATE "default",
"site_error" text COLLATE "default",
"site_data" text COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table headers
-- ----------------------------
ALTER TABLE "public"."headers" ADD PRIMARY KEY ("headerid");

-- ----------------------------
-- Primary Key structure for table sites
-- ----------------------------
ALTER TABLE "public"."sites" ADD PRIMARY KEY ("site_id");

-- ----------------------------
-- Foreign Key structure for table "public"."headers"
-- ----------------------------
ALTER TABLE "public"."headers" ADD FOREIGN KEY ("siteid") REFERENCES "public"."sites" ("site_id") ON DELETE NO ACTION ON UPDATE NO ACTION;
