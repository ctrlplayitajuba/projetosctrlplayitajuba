#include "Servo.h"
#include "StackArray.h"

//TIPOS DE DADOS
enum Direction {
  LEFT = -90,
  RIGHT = 90,
  FORWARDS = 0,
  BACKWARDS = 180
};

struct Movement {
  float moveTime;
  Direction rotation;
  bool FREE_FORWARD;
  bool FREE_LEFT;
  bool FREE_RIGHT;
};

//FUNÇÕES
void Move();
void Stop();
void MoveForSeconds(float);
void Rotate(int);

//PINOS DO ARDUINO
#define WHEEL_RIGHT           8           // pino da roda direita do carro
#define WHEEL_LEFT            9           // pino da roda esquerda do carro

//CONSTANTES
#define ENGINE_FORWARDS       0           // valor para ligar o motor girando para frente
#define ENGINE_BACKWARDS      100          // valor para ligar o motor girando para trás
#define ENGINE_OFF            90          // valor para desligar o motor

//VARIÁVEIS
Servo servoWheelRight;
Servo servoWheelLeft;
StackArray <Movement> movementStack;

void setup() {
  // put your setup code here, to run once:
  servoWheelRight.attach(WHEEL_RIGHT);
  servoWheelLeft.attach(WHEEL_LEFT);
  Serial.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
  MoveForSeconds(2);
  delay(2000);
  Rotate(LEFT);
  delay(2000);
  Rotate(RIGHT);
  delay(2000);
}

void Move(){
  servoWheelRight.write(ENGINE_FORWARDS);
  servoWheelLeft.write(ENGINE_FORWARDS);
}

void Stop(){
  servoWheelRight.write(ENGINE_OFF);
  servoWheelLeft.write(ENGINE_OFF);
}

void MoveForSeconds(float seconds){
  Move();
  delay(1000 * seconds);
  Stop();
}

void Rotate (int rotation){
  switch (rotation){
    case LEFT:
      servoWheelRight.write(ENGINE_FORWARDS);
      servoWheelLeft.write(ENGINE_BACKWARDS);
      delay(500);
      Stop();
      break;
    case RIGHT:
      servoWheelRight.write(ENGINE_BACKWARDS);
      servoWheelLeft.write(ENGINE_FORWARDS);
      delay(500);
      Stop();
      break;
    case FORWARDS:
      break;
    case BACKWARDS:
      servoWheelRight.write(ENGINE_FORWARDS);
      servoWheelLeft.write(ENGINE_BACKWARDS);
      delay(1000);
      Stop();
      break;
    default:
      break;
  }
}

  void PileMovement(int turn, bool canTurnLeft, bool canTurnRight, bool canGoForward) {
    Movement movement = new Movement();
    movement.rotation = turn;
    movement.moveTime = moveTime;
    movement.FREE_LEFT = canTurnLeft;
    movement.FREE_RIGHT = canTurnRight;
    movement.FREE_FORWARD = canGoForward;
    movementStack.Push(movement);
    canMove = true;
  }
