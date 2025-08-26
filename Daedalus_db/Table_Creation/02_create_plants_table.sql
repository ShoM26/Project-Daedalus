CREATE TABLE plants
(
    plant_id INT AUTO_INCREMENT PRIMARY KEY,
    scientific_name VARCHAR(150) NOT NULL UNIQUE,
    familiar_name VARCHAR(150) NOT NULL,
    moisture_low_range DECIMAL(5,2) NOT NULL,
    moisture_high_range DECIMAL(5,2) NOT NULL,
    fun_fact TEXT
)