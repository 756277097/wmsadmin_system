/*
 Navicat Premium Data Transfer

 Source Server         : localhost
 Source Server Type    : MySQL
 Source Server Version : 80021
 Source Host           : localhost:3307
 Source Schema         : wms

 Target Server Type    : MySQL
 Target Server Version : 80021
 File Encoding         : 65001

 Date: 28/11/2025 14:45:14
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for buttons
-- ----------------------------
DROP TABLE IF EXISTS `buttons`;
CREATE TABLE `buttons`  (
  `Id` int(0) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MenuId` int(0) NOT NULL,
  `ButtonType` int(0) NOT NULL DEFAULT 0 COMMENT '0-查看，1-新增，2-编辑，3-删除，4-其他',
  `Sort` int(0) NOT NULL DEFAULT 0,
  `IsEnabled` tinyint(1) NULL DEFAULT 1,
  `CreateTime` datetime(0) NOT NULL,
  `UpdateTime` datetime(0) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `idx_menu`(`MenuId`) USING BTREE,
  CONSTRAINT `buttons_ibfk_1` FOREIGN KEY (`MenuId`) REFERENCES `menus` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of buttons
-- ----------------------------
INSERT INTO `buttons` VALUES (1, '查看', 'User:View', 7, 0, 0, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (2, '新增', 'User:Add', 7, 1, 1, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (3, '编辑', 'User:Edit', 7, 2, 2, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (4, '删除', 'User:Delete', 7, 3, 3, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (5, '查看', 'Role:View', 8, 0, 0, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (6, '新增', 'Role:Add', 8, 1, 1, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (7, '编辑', 'Role:Edit', 8, 2, 2, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (8, '删除', 'Role:Delete', 8, 3, 3, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (9, '查看', 'Menu:View', 9, 0, 0, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (10, '新增', 'Menu:Add', 9, 1, 1, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (11, '编辑', 'Menu:Edit', 9, 2, 2, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (12, '删除', 'Menu:Delete', 9, 3, 3, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (13, '查看', 'Warehouse:View', 10, 0, 0, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (14, '新增', 'Warehouse:Add', 10, 1, 1, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (15, '编辑', 'Warehouse:Edit', 10, 2, 2, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (16, '删除', 'Warehouse:Delete', 10, 3, 3, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (17, '查看', 'Inventory:View', 11, 0, 0, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (18, '导出', 'Inventory:Export', 11, 4, 1, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `buttons` VALUES (19, '盘点', 'Inventory:Stocktake', 11, 4, 2, 1, '2025-11-28 13:50:46', NULL);

-- ----------------------------
-- Table structure for menus
-- ----------------------------
DROP TABLE IF EXISTS `menus`;
CREATE TABLE `menus`  (
  `Id` int(0) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ParentId` int(0) NULL DEFAULT NULL,
  `MenuType` int(0) NOT NULL DEFAULT 0 COMMENT '0-内部页面，1-外部链接',
  `Path` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `Icon` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `Sort` int(0) NOT NULL DEFAULT 0,
  `IsEnabled` tinyint(1) NULL DEFAULT 1,
  `CreateTime` datetime(0) NOT NULL,
  `UpdateTime` datetime(0) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `Code`(`Code`) USING BTREE,
  INDEX `idx_code`(`Code`) USING BTREE,
  INDEX `idx_parent`(`ParentId`) USING BTREE,
  CONSTRAINT `menus_ibfk_1` FOREIGN KEY (`ParentId`) REFERENCES `menus` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of menus
-- ----------------------------
INSERT INTO `menus` VALUES (4, '系统管理', 'System', NULL, 0, '#', '⚙️', 1, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `menus` VALUES (5, '业务管理', 'Business', NULL, 0, '#', '📦', 2, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `menus` VALUES (6, '报表中心', 'Report', NULL, 0, '#', '📈', 3, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `menus` VALUES (7, '用户管理', 'User', 4, 0, '/User', '👤', 1, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `menus` VALUES (8, '角色管理', 'Role', 4, 0, '/Role', '🔐', 2, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `menus` VALUES (9, '菜单管理', 'Menu', 4, 0, '/Menu', '📋', 3, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `menus` VALUES (10, '仓库管理', 'Warehouse', 5, 0, '/Warehouse', '🏭', 1, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `menus` VALUES (11, '库存管理', 'Inventory', 5, 0, '/Inventory', '📊', 2, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `menus` VALUES (12, '入库管理', 'Inbound', 5, 0, '/Inbound', '📥', 3, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `menus` VALUES (13, '出库管理', 'Outbound', 5, 0, '/Outbound', '📤', 4, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `menus` VALUES (14, '日报表', 'DailyReport', 6, 0, '/Report/Daily', '📅', 1, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `menus` VALUES (15, '月报表', 'MonthlyReport', 6, 0, '/Report/Monthly', '📆', 2, 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `menus` VALUES (16, '库存报表', 'InventoryReport', 6, 0, '/Report/Inventory', '📊', 3, 1, '2025-11-28 13:50:46', NULL);

-- ----------------------------
-- Table structure for rolepermissions
-- ----------------------------
DROP TABLE IF EXISTS `rolepermissions`;
CREATE TABLE `rolepermissions`  (
  `Id` int(0) NOT NULL AUTO_INCREMENT,
  `RoleId` int(0) NOT NULL,
  `MenuId` int(0) NULL DEFAULT NULL,
  `ButtonId` int(0) NULL DEFAULT NULL,
  `PermissionType` int(0) NOT NULL COMMENT '0-菜单，1-按钮',
  `CreateTime` datetime(0) NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `RoleId`(`RoleId`) USING BTREE,
  INDEX `MenuId`(`MenuId`) USING BTREE,
  INDEX `ButtonId`(`ButtonId`) USING BTREE,
  CONSTRAINT `rolepermissions_ibfk_1` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `rolepermissions_ibfk_2` FOREIGN KEY (`MenuId`) REFERENCES `menus` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `rolepermissions_ibfk_3` FOREIGN KEY (`ButtonId`) REFERENCES `buttons` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of rolepermissions
-- ----------------------------
INSERT INTO `rolepermissions` VALUES (1, 17, 4, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (2, 17, 5, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (3, 17, 6, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (4, 17, 7, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (5, 17, 8, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (6, 17, 9, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (7, 17, 10, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (8, 17, 11, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (9, 17, 12, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (10, 17, 13, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (11, 17, 14, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (12, 17, 15, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (13, 17, 16, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (14, 17, 7, 1, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (15, 17, 7, 2, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (16, 17, 7, 3, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (17, 17, 7, 4, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (18, 17, 8, 5, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (19, 17, 8, 6, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (20, 17, 8, 7, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (21, 17, 8, 8, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (22, 17, 9, 9, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (23, 17, 9, 10, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (24, 17, 9, 11, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (25, 17, 9, 12, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (26, 17, 10, 13, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (27, 17, 10, 14, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (28, 17, 10, 15, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (29, 17, 10, 16, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (30, 17, 11, 17, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (31, 17, 11, 18, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (32, 17, 11, 19, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (33, 18, 4, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (34, 18, 7, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (35, 18, 8, NULL, 0, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (36, 18, 7, 1, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (37, 18, 7, 2, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (38, 18, 7, 3, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (39, 18, 8, 5, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (40, 18, 8, 6, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (41, 18, 8, 7, 1, '2025-11-28 13:50:46');
INSERT INTO `rolepermissions` VALUES (42, 19, 5, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (43, 19, 10, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (44, 19, 12, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (45, 19, 13, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (46, 19, 10, 13, 1, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (47, 19, 10, 14, 1, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (48, 19, 10, 15, 1, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (49, 19, 10, 16, 1, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (50, 20, 5, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (51, 20, 11, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (52, 20, 6, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (53, 20, 16, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (54, 20, 11, 17, 1, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (55, 20, 11, 18, 1, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (56, 20, 11, 19, 1, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (57, 21, 4, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (58, 21, 5, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (59, 21, 6, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (60, 21, 7, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (61, 21, 8, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (62, 21, 9, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (63, 21, 10, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (64, 21, 11, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (65, 21, 12, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (66, 21, 13, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (67, 21, 14, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (68, 21, 15, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (69, 21, 16, NULL, 0, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (70, 21, 7, 1, 1, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (71, 21, 8, 5, 1, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (72, 21, 9, 9, 1, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (73, 21, 10, 13, 1, '2025-11-28 13:50:47');
INSERT INTO `rolepermissions` VALUES (74, 21, 11, 17, 1, '2025-11-28 13:50:47');

-- ----------------------------
-- Table structure for roles
-- ----------------------------
DROP TABLE IF EXISTS `roles`;
CREATE TABLE `roles`  (
  `Id` int(0) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `IsEnabled` tinyint(1) NULL DEFAULT 1,
  `CreateTime` datetime(0) NOT NULL,
  `UpdateTime` datetime(0) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `Code`(`Code`) USING BTREE,
  INDEX `idx_code`(`Code`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of roles
-- ----------------------------
INSERT INTO `roles` VALUES (17, '超级管理员', 'SuperAdmin', '拥有所有权限', 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `roles` VALUES (18, '普通管理员', 'Admin', '普通管理权限，可管理用户和角色', 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `roles` VALUES (19, '仓库管理员', 'WarehouseManager', '负责仓库日常管理', 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `roles` VALUES (20, '库存管理员', 'InventoryManager', '负责库存管理', 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `roles` VALUES (21, '只读用户', 'ReadOnly', '只能查看，无操作权限', 1, '2025-11-28 13:50:46', NULL);

-- ----------------------------
-- Table structure for userroles
-- ----------------------------
DROP TABLE IF EXISTS `userroles`;
CREATE TABLE `userroles`  (
  `Id` int(0) NOT NULL AUTO_INCREMENT,
  `UserId` int(0) NOT NULL,
  `RoleId` int(0) NOT NULL,
  `CreateTime` datetime(0) NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `uk_user_role`(`UserId`, `RoleId`) USING BTREE,
  INDEX `RoleId`(`RoleId`) USING BTREE,
  CONSTRAINT `userroles_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `userroles_ibfk_2` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of userroles
-- ----------------------------
INSERT INTO `userroles` VALUES (1, 21, 17, '2025-11-28 13:50:46');
INSERT INTO `userroles` VALUES (2, 22, 18, '2025-11-28 13:50:46');
INSERT INTO `userroles` VALUES (3, 23, 18, '2025-11-28 13:50:46');
INSERT INTO `userroles` VALUES (4, 24, 19, '2025-11-28 13:50:46');
INSERT INTO `userroles` VALUES (5, 25, 20, '2025-11-28 13:50:46');

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS `users`;
CREATE TABLE `users`  (
  `Id` int(0) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `RealName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `Email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `Phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `IsEnabled` tinyint(1) NULL DEFAULT 1,
  `CreateTime` datetime(0) NOT NULL,
  `UpdateTime` datetime(0) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `UserName`(`UserName`) USING BTREE,
  INDEX `idx_username`(`UserName`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of users
-- ----------------------------
INSERT INTO `users` VALUES (21, 'admin', 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=', '系统管理员', 'admin@wms.com', '13800138000', 1, '2025-11-28 13:50:46', '2025-11-28 14:35:34');
INSERT INTO `users` VALUES (22, 'test', 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=', '测试用户', 'test@wms.com', '13800138001', 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `users` VALUES (23, 'user', 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=', '普通用户', 'user@wms.com', '13800138002', 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `users` VALUES (24, 'warehouse', 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=', '仓库管理员', 'warehouse@wms.com', '13800138003', 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `users` VALUES (25, 'inventory', 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=', '库存管理员', 'inventory@wms.com', '13800138004', 1, '2025-11-28 13:50:46', NULL);
INSERT INTO `users` VALUES (26, 'aaa', 'z64mKIvYLhqXZpt3IEcM85Toew5TvdflhAVYBcxjAB8=', 'aaaaa', NULL, NULL, 1, '2025-11-28 14:35:44', '2025-11-28 14:35:51');

SET FOREIGN_KEY_CHECKS = 1;
