CREATE TABLE user_plants
(
    user_plant_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    plant_id INT NOT NULL,
    device_id INT NOT NULL,
    date_added TIMESTAMP DEFAULT CURRENT_TIMESTAMP(),

    CONSTRAINT fk_userplants_user FOREIGN KEY (user_id)
        REFERENCES users(user_id) ON DELETE CASCADE,
    CONSTRAINT fk_userplants_plant FOREIGN KEY (plant_id)
        REFERENCES plants(plant_id) ON DELETE RESTRICT,
    CONSTRAINT fk_userplants_device FOREIGN KEY (device_id)
        REFERENCES devices(device_id) ON DELETE CASCADE,
    UNIQUE (user_id, device_id)
);