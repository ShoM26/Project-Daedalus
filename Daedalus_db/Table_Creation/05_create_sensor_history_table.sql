CREATE TABLE sensor_history
(
    time_stamp TIMESTAMP NOT NULL,
    device_id INT NOT NULL,
    moisture_level DECIMAL(5,2) NOT NULL,

    CONSTRAINT pk_sensor_history PRIMARY KEY (time_stamp, device_id),
    CONSTRAINT fk_sensor_history FOREIGN KEY (device_id)
        REFERENCES devices(device_id) ON DELETE CASCADE
)