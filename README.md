# NanoDrone

NanoDrone is the software running on a custom built drone.  
The project is a windows 10 IOT project, running on a raspberry pi 3 model B

## Parts
The parts used for the build are: 
* [Raspberry pi 3 model B](https://www.raspberrypi.org/products/raspberry-pi-3-model-b/)
* [Adafruit 9-DOF Absolute Orientation IMU Fusion Breakout - BNO055](https://www.adafruit.com/product/2472)
* [Adafruit 16-channel PWM / servo hat for Raspberry pi](https://www.adafruit.com/product/2327)
* [Adafruit Ultimate GPS Breakout](https://www.adafruit.com/product/746)
* Several ultrasonic distance sensors
* Several LiPo batteries
* Some wires & resistors
* Rise RXS270 drone kit with ESCs and motors  

The motors, frame and ESCs can be swapped out with anything else.  
Changes to the PWM frequency might be required as result of swapping ESCs though.

## Libraries used
Golaat's [Adafruit.Pwm](https://github.com/golaat/Adafruit.Pwm) library was used for the PWM servo hat.
