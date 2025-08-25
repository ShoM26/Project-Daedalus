CREATE TABLE devices
(
    device_id INT AUTO_INCREMENT PRIMARY KEY,
    device_name VARCHAR(150) NOT NULL,
    connection_type ENUM('USB','Bluetooth','WiFi') NOT NULL,
    connection_address VARCHAR(250) NOT NULL UNIQUE,
    last_seen TIMESTAMP NULL,
    status ENUM('Active', 'Inactive','Disconnected'),
)