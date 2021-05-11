﻿CREATE TABLE products (
	id CHAR(12) NOT NULL,
	PRIMARY KEY(id),
	dateOfFirstOffer DATE,
	title CHAR(200),
	gender ENUM('m', 'f'),
	brand CHAR(30),
	department CHAR(30),
	category CHAR(30),
	silhouette CHAR(30),
	product CHAR(30),
	occasion CHAR(30)
);

CREATE TABLE prices (
	id INT UNSIGNED NOT NULL AUTO_INCREMENT,
	PRIMARY KEY (id),
	price DECIMAL(7, 2),
	type ENUM('s', 'r', 'h', 'b', 'n'),
	label ENUM('s', 'r', 'o', 'c', 'g', 'n', 'p'),
	num1 DECIMAL(7, 2),
	num2 DECIMAL(7, 2)
);

/*
Type enum:
- s = single ($10)
- r = range ($10 - $20)
- h = hybrid ($5 or 2 / $9)
- b = bulk (3 / $12)
- n = no price 

Label enum:
- s = sale
- r = regular
- o = original
- c = clearance
- g = group
- n = none (there is a price but it doesn't have a label)
- p = no price
*/


CREATE TABLE offers (
	id INT UNSIGNED NOT NULL AUTO_INCREMENT,
	PRIMARY KEY (id),
	productId CHAR(12) NOT NULL,
	FOREIGN KEY (productId) REFERENCES products(id),
	dateTime DATETIME,
	stars FLOAT(5),
	reviews MEDIUMINT UNSIGNED,
	primaryPriceId INT UNSIGNED NOT NULL,
	FOREIGN KEY (primaryPriceId) REFERENCES prices(id),
	alternatePriceId INT UNSIGNED NOT NULL,
	FOREIGN KEY (alternatePriceId) REFERENCES prices(id)
);

INSERT INTO products VALUES ('c17769d0', '2021-04-30', 'A Great Title for a product', 'Levi', 'm', 'clothing', 'tops', 't-shirt', null, null);
INSERT INTO prices (price, type, label, num1, num2) VALUES (24.56, 'Range', 'Clearance', 20.00, 50.00);
INSERT INTO offers (productId, dateTime, stars, reviews, primaryPriceId, alternatePriceId) VALUES ('key', '2021-04-30 23:24:01', 4.2, 672, [key], [key]);

SELECT LAST_INSERT_ID();