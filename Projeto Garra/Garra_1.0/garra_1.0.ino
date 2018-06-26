#include "Servo.h"

Servo servo_garra;
Servo servo_punhoGiratorio;
Servo servo_punho;
Servo servo_cotovelo;
Servo servo_ombro;
Servo servo_base;

int vx1 = 0;

int vsb = 0;

int vx2 = 0;

int vx3 = 0;
int vy3 = 0;

int vsg = 0;

int newvsg;
int newvsb;

void setup()
{
  servo_garra.attach(11);
  servo_punhoGiratorio.attach(9);
  servo_punho.attach(10);
  servo_cotovelo.attach(8);
  servo_ombro.attach(7);
  servo_base.attach(6);

  Serial.begin(9600);
}
void loop()
{
  /**/                            // LEITURA DOS VALORES ANALÓGICOS
  vsb = analogRead(2);    //[V]alor do Potenciômetro para o [S]ervo da [B]ase


  vx1 = analogRead(1);    //[V]alor do eixo [X] [1]

  vx2 = analogRead(0);    //[V]alor do eixo [X] [2]

  vx3 = analogRead(3);    //[V]alor do eixo [X] [3]
  vy3 = analogRead(4);    //[V]alor do eixo [Y] [3]

  vsg = analogRead(5);    //[V]alor do Potenciômetro para o [S]ervo da [G]arra
  // *********************************************************************************************//
  vsg = map(vsg, 0, 1023, 10, 90);//MAPEAMENTO DO VALOR DO POTENCIOMETRO PARA ANGULO DO SERVO

  vsb = map(vsb, 0, 1023, 10, 180);

  // *********************************************************************************************//
  if (vx1 < 300 && vx1 > 200)                  //MOVIMENTO SERVO OMBRO
  {
    Serial.println(" VALOR DE Y 3 MENOR QUE 300");
    for(int i = 99; i < 102; i++)
    {
      servo_ombro.write(i);
    }
  }else if(vx1 <= 200)
  {
    servo_ombro.write(102);
  }
  if (vx1 > 700 && vx1 < 800)
  {
    Serial.println(" VALOR DE Y 3 MAIOR QUE 700");
    for(int i = 88; i > 86; i--)
    {
      servo_ombro.write(i);
    }
  }else if (vx1 >= 800)
  {
    servo_ombro.write(86);
  }
  if(vx1 >=300 && vx1 <= 700){
    servo_ombro.write(90);
  }
  // *********************************************************************************************//
  servo_base.write(vsb);              //MOVIMENTO SERVO BASE
  if (vsb > newvsb + 10 || vsb < newvsb - 10)
  {
    Serial.println(" POTENCIOMETRO BASE:");
    Serial.print(vsb);
  }
  delay(30);
  newvsb = vsb;
  // *********************************************************************************************//                
  if (vx2 < 300 && vx2 > 200)                  //MOVIMENTO SERVO COTOVELO
  {
    Serial.println(" VALOR DE Y 3 MENOR QUE 300");
    //100 -> 102
    for(int i = 99; i < 102; i++)
    {
      servo_cotovelo.write(i);
    }
  }else if(vx2 <= 200)
  {
    servo_cotovelo.write(102);
  }
  if (vx2 > 700 && vx2 < 800)
  {
    Serial.println(" VALOR DE Y 3 MAIOR QUE 700");
    //87 -> 85
    for(int i = 88; i > 87; i--)
    {
      servo_cotovelo.write(i);
    }
  }else if (vx2 >= 800)
  {
    servo_cotovelo.write(87);
  }
  if(vx2 >=300 && vx2 <= 700){
    servo_cotovelo.write(90);
  }
  // *********************************************************************************************//
  if (vx3 < 300 && vx3 > 200)                  //MOVIMENTO SERVO PUNHO FRENTE/TRÁS
  {
    Serial.println(" VALOR DE Y 3 MENOR QUE 300");
    for(int i = 97; i < 99; i++)
    {
      servo_punho.write(i);
    }
  }else if(vx3 <= 200)
  {
    servo_punho.write(99);
  }
  if (vx3 > 700 && vx3 < 800)
  {
    Serial.println(" VALOR DE Y 3 MAIOR QUE 700");
    for(int i = 87; i > 85; i--)
    {
      servo_punho.write(i);
    }
  }else if (vx3 >= 800)
  {
    servo_punho.write(85);
  }
  if(vx3 >=300 && vx3 <= 700){
    servo_punho.write(90);
  }
  // *********************************************************************************************//
  if (vy3 < 300 && vy3 > 200)                  //MOVIMENTO SERVO PUNHO GIRATÓRIO
  {
    Serial.println(" VALOR DE Y 3 MENOR QUE 300");
    for(int i = 90; i < 96; i++)
    {
      servo_punhoGiratorio.write(i);
    }
  }else if(vy3 <= 200)
  {
    servo_punhoGiratorio.write(100);
  }
  if (vy3 > 700 && vy3 < 800)
  {
    Serial.println(" VALOR DE Y 3 MAIOR QUE 700");
    for(int i = 87; i > 85; i--)
    {
      servo_punhoGiratorio.write(i);
    }
  }else if (vy3 >= 800)
  {
    servo_punhoGiratorio.write(85);
  }
  if(vy3 >=300 && vy3 <= 700){
    servo_punhoGiratorio.write(90);
  }
  // *********************************************************************************************//
  servo_garra.write(vsg);              //MOVIMENTO SERVO GARRA
  if (vsg > newvsg + 10 || vsg < newvsg - 10)
  {
    Serial.println(" POTENCIOMETRO GARRA:");
    Serial.print(vsg);
  }
  delay(30);
  newvsg = vsg;

//  for(int i = 100; i < 110; i++)
//  {
//    servo_ombro.write(i);
//    Serial.println(i);
//    delay(2000);
//  }
}
