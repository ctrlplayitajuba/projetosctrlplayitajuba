#include <Servo.h>

/* Servo control for AL5D arm */

/* Arm dimensions( mm ) */
#define BASE_HGT 67.31      //base hight 2.65"
#define HUMERUS 146.05      //shoulder-to-elbow "bone" 5.75"
#define ULNA 187.325        //elbow-to-wrist "bone" 7.375"
#define GRIPPER 100.00          //gripper (incl.heavy duty wrist rotate mechanism) length 3.94"

#define ftl(x) ((x)>=0?(long)((x)+0.5):(long)((x)-0.5))  //float to long conversion

/* Servo names/numbers */
/* Base servo HS-485HB */
#define BAS_SERVO_PIN 0
/* Shoulder Servo HS-5745-MG */
#define SHL_SERVO_PIN 1
/* Elbow Servo HS-5745-MG */
#define ELB_SERVO_PIN 2
/* Wrist servo HS-645MG */
#define WRI_SERVO_PIN 3 
/* Wrist rotate servo HS-485HB */
#define WRO_SERVO_PIN 4
/* Gripper servo HS-422 */
#define GRI_SERVO_PIN 5

/* pre-calculations */
float hum_sq = HUMERUS * HUMERUS;
float uln_sq = ULNA * ULNA;

ServoShield servos;                       //ServoShield object
Servo  BAS_SERVO;
Servo  SHL_SERVO;
Servo  ELB_SERVO;
Servo  WRI_SERVO;
Servo  WRO_SERVO;
Servo  GRI_SERVO;

void setup()
{
  BAS_SERVO.attach(BAS_SERVO_PIN);
  SHL_SERVO.attach(SHL_SERVO_PIN);
  ELB_SERVO.attach(ELB_SERVO_PIN);
  WRI_SERVO.attach(WRI_SERVO_PIN);
  WRO_SERVO.attach(WRO_SERVO_PIN);
  GRI_SERVO.attach(GRI_SERVO_PIN);
  
  /**/
  servo_park();
  Serial.begin( 115200 );
  Serial.println("Start");
  delay( 500 );
}

void loop()
{

  //zero_x();
  //line();
  circle();
}

/* arm positioning routine utilizing inverse kinematics */
/* z is height, y is distance from base center out, x is side to side. y,z can only be positive */
//void set_arm( uint16_t x, uint16_t y, uint16_t z, uint16_t grip_angulo )
void set_arm( float x, float y, float z, float grip_angulo_d )
{
  float grip_angulo_r = radians( grip_angulo_d );    //grip angulo in radians for use in calculations
  /* Base angulo and radial distance from x,y coordinates */
  float bas_angulo_r = atan2( x, y );
  float rdist = sqrt(( x * x ) + ( y * y ));
  /* rdist is y coordinate for the arm */
  y = rdist;
  /* Grip offsets calculated based on grip angulo */
  float grip_off_z = ( sin( grip_angulo_r )) * GRIPPER;
  float grip_off_y = ( cos( grip_angulo_r )) * GRIPPER;
  /* Wrist position */
  float wrist_z = ( z - grip_off_z ) - BASE_HGT;
  float wrist_y = y - grip_off_y;
  /* Shoulder to wrist distance ( AKA sw ) */
  float s_w = ( wrist_z * wrist_z ) + ( wrist_y * wrist_y );
  float s_w_sqrt = sqrt( s_w );
  /* s_w angulo to ground */
  //float a1 = atan2( wrist_y, wrist_z );
  float a1 = atan2( wrist_z, wrist_y );
  /* s_w angulo to humerus */
  float a2 = acos((( hum_sq - uln_sq ) + s_w ) / ( 2 * HUMERUS * s_w_sqrt ));
  /* shoulder angulo */
  float shl_angulo_r = a1 + a2;
  float shl_angulo_d = degrees( shl_angulo_r );
  /* elbow angulo */
  float elb_angulo_r = acos(( hum_sq + uln_sq - s_w ) / ( 2 * HUMERUS * ULNA ));
  float elb_angulo_d = degrees( elb_angulo_r );
  float elb_angulo_dn = -( 180.0 - elb_angulo_d );
  /* wrist angulo */
  float wri_angulo_d = ( grip_angulo_d - elb_angulo_dn ) - shl_angulo_d;

  /* Servo pulses */
  float bas_servopulse = 1500.0 - (( degrees( bas_angulo_r )) * 11.11 );
  float shl_servopulse = 1500.0 + (( shl_angulo_d - 90.0 ) * 6.6 );
  float elb_servopulse = 1500.0 -  (( elb_angulo_d - 90.0 ) * 6.6 );
  float wri_servopulse = 1500 + ( wri_angulo_d  * 11.1 );

  /* Set servos */
  BAS_SERVO.writeMicroseconds(  bas_servopulse ); 
  WRI_SERVO.writeMicroseconds(  wri_servopulse );
  SHL_SERVO.writeMicroseconds(  shl_servopulse );
  ELB_SERVO.writeMicroseconds(  elb_servopulse );
}

/* move servos to parking position */
void servo_park()
{
  BAS_SERVO.writeMicroseconds( 1715 );
  SHL_SERVO.writeMicroseconds( 2100 );
  ELB_SERVO.writeMicroseconds( 2100 );
  WRI_SERVO.writeMicroseconds( 1800 );
  WRO_SERVO.writeMicroseconds(  600 );
  GRI_SERVO.writeMicroseconds(  900 );
  return;
}

void zero_x()
{
  for ( double eixo_y = 150.0; eixo_y < 356.0; eixo_y += 1 ) {
    set_arm( 0, eixo_y, 127.0, 0 );
    delay( 10 );
  }
  for ( double eixo_y = 356.0; eixo_y > 150.0; eixo_y -= 1 ) {
    set_arm( 0, eixo_y, 127.0, 0 );
    delay( 10 );
  }
}

/* moves arm in a straight line */
void line()
{
  for ( double eixo_x = -100.0; eixo_x < 100.0; eixo_x += 0.5 ) {
    set_arm( eixo_x, 250, 100, 0 );
    delay( 10 );
  }
  for ( float eixo_x = 100.0; eixo_x > -100.0; eixo_x -= 0.5 ) {
    set_arm( eixo_x, 250, 100, 0 );
    delay( 10 );
  }
}

void circle()
{
#define RAIO 80.0
  //float angulo = 0;
  float eixo_z, eixo_y;
  for ( float angulo = 0.0; angulo < 360.0; angulo += 1.0 ) {
    eixo_y = RAIO * sin( radians( angulo )) + 200;
    eixo_z = RAIO * cos( radians( angulo )) + 200;
    set_arm( 0, eixo_y, eixo_z, 0 );
    delay( 1 );
  }
}
