#include "Servo.h"
#include "StackArray.h"

//----------------------------------------TIPOS DE DADOS---------------------------------------------//
struct Movement {
  float moveTime;
  int rotation;
  bool FREE_FORWARD;
  bool FREE_LEFT;
  bool FREE_RIGHT;
};
//----------------------------------------------------------------------------------------------------//
//-----------------------------------------PINOS DO ARDUINO-------------------------------------------//
#define WHEEL_RIGHT           8           // pino da roda direita do carro
#define WHEEL_LEFT            9           // pino da roda esquerda do carro
//----------------------------------------------------------------------------------------------------//
//--------------------------------------------CONSTANTES----------------------------------------------//
#define ENGINE_FORWARD        0           // valor para ligar o motor girando para frente
#define ENGINE_BACKWARD       100         // valor para ligar o motor girando para trás
#define ENGINE_OFF            90          // valor para desligar o motor
#define LEFT                  0
#define RIGHT                 1
#define FORWARD               2
#define BACKWARD              3
//----------------------------------------------------------------------------------------------------//
//--------------------------------------------VARIÁVEIS-----------------------------------------------//
Servo servoWheelRight;
Servo servoWheelLeft;
StackArray <Movement> movementStack;
float moveTime;
bool reachedEnd;
int initialTurn;
Movement *pathToStart;
Movement *pathToEnd;
//----------------------------------------------------------------------------------------------------//

void setup() {
  // put your setup code here, to run once:
  servoWheelRight.attach(WHEEL_RIGHT);
  servoWheelLeft.attach(WHEEL_LEFT);
  Serial.begin(9600);

  reachedEnd = false;
  initialTurn = FORWARD;
}

void loop() {
//  //Detection();
//  //bifurcação com todos os lados livres
//  DetectionTreatment(true, true, true);
//  delay(5000);
//  //só pode ir para frente
//  DetectionTreatment(false, false, true);
//  delay(5000);
//  //só pode virar para a esquerda
//  DetectionTreatment(true, false, false);
//  delay(5000);
//  //pode virar para esquerda e direita
//  DetectionTreatment(true, true, false);
//  delay(5000);
//  //dead end
//  DetectionTreatment(false, false, false);
//  delay(5000);
//  //dead end
//  DetectionTreatment(false, false, false);
//  delay(5000);
//  //Move();
}

void Detection() {
  bool canTurnLeft = false;
  bool canTurnRight = false;
  bool canGoForward = false;

  //SENSORES xD

  if ((canTurnLeft || canTurnRight) && !canGoForward) {
    Stop();
  }

  DetectionTreatment(canTurnLeft, canTurnRight, canGoForward);
}

void DetectionTreatment(bool canTurnLeft, bool canTurnRight, bool canGoForward) {
  if (canGoForward && !canTurnLeft && !canTurnRight) {
    Serial.println("SEGUE EM FRENTE");
    return;
  }
  else {
    PileMovement(initialTurn, canTurnLeft, canTurnRight, canGoForward);
    if (reachedEnd)
      MakePaths ();
    if (!canGoForward && canTurnRight) {
      Serial.println("VIROU PARA A DIREITA");
      Rotate(RIGHT);
      initialTurn = RIGHT;
    }
    else if (!canGoForward && canTurnLeft) {
      Serial.println("VIROU PARA A ESQUERDA");
      Rotate(LEFT);
      initialTurn = LEFT;
    }
    else if (canGoForward && (canTurnLeft || canTurnRight)) {
      Serial.println("EMPILHOU E CONTINUOU PARA FRENTE");
      Rotate (FORWARD);
      initialTurn = FORWARD;
    }
    else if (!reachedEnd){
      Serial.println("VOLTANDO");
      Rewind ();
    }
  }
}

void Rewind() {
  while (movementStack.count() > 0) {
    if (movementStack.peek ().FREE_RIGHT) {
      Serial.println("ACHOU DIREITA LIVRE");
      Rotate (RIGHT);
      initialTurn = RIGHT;
      break;
    } else if (movementStack.peek ().FREE_LEFT) {
      Serial.println("ACHOU ESQUERDA LIVRE");
      Rotate (LEFT);
      initialTurn = LEFT;
      break;
    } else {
      if (movementStack.count() < 1)
        break;
      else {
        Serial.println("RETORNANDO MOVIMENTO");
        int rotation = movementStack.peek ().rotation;
        ReverseMovement ();
        if (movementStack.count() > 0) {
          Movement movement;
          movement = movementStack.pop ();
          movement.FREE_FORWARD = false;
          if (rotation == RIGHT)
            movement.FREE_RIGHT = false;
          else if (rotation == LEFT)
            movement.FREE_LEFT = false;
          movementStack.push (movement);
        }
      }
    }
  }
}

//Reverte o último movimento da pilha de movimentos
void ReverseMovement() {
  if (movementStack.count() != 0) {             //se tiver elementos na pilha:
    Movement movement = movementStack.pop ();     //pega último movimento da pilha
    MoveBackForSeconds (movement.moveTime);       //anda para trás com o carro por movement.moveTime segundos
    if (movement.rotation == FORWARD)             //se a rotação inicial do carro for para frente:
      Rotate (FORWARD);                             //mantém o carro olhando para frente
    else if (movement.rotation == LEFT)           //se a rotação inicial do carro for para a esquerda:
      Rotate (RIGHT);                               //rotaciona o carro para a direita
    else if (movement.rotation == RIGHT)          //se a rotação inicial do carro for para a direita:
      Rotate (LEFT);                                //rotaciona o carro para a esquerda
  }
}

//Faz as listas com os caminhos do final do labirinto até o começo e vice-versa
void MakePaths() {
  
}

//Faz o carro se mover para frente
void Move() {
  servoWheelRight.write(ENGINE_FORWARD); //liga o motor da roda direita
  servoWheelLeft.write(ENGINE_FORWARD);  //liga o motor da roda esquerda
}

//Para o movimento do carro
void Stop() {
  servoWheelRight.write(ENGINE_OFF); //desliga o motor da roda direita
  servoWheelLeft.write(ENGINE_OFF);  //desliga o motor da roda esquerda
}

//Faz o carro andar para frente por (float seconds) segundos
void MoveForSeconds(float seconds) {
  Move();                   //liga os motores das rodas
  delay(1000 * seconds);    //espera (float seconds) segundos
  Stop();                   //para as rodas
}

//Faz o carro andar para trás por (float seconds) segundos
void MoveBackForSeconds(float seconds) {
  servoWheelRight.write(ENGINE_BACKWARD);  //gira a roda direita para trás
  servoWheelLeft.write(ENGINE_BACKWARD);   //gira a roda esquerda para trás
  delay(1000 * seconds);                   //espera (float seconds) segundos
  Stop();                                  //para as rodas
}

//Rotaciona o carro para a direção de (int rotation)
void Rotate (int rotation) {
  switch (rotation) {
    case LEFT:
      servoWheelRight.write(ENGINE_FORWARD);  //gira a roda direita para frente
      servoWheelLeft.write(ENGINE_BACKWARD);  //gira a roda esquerda para trás
      delay(500);                             //tempo necessário para efetuar a rotação
      Stop();                                 //para as rodas
      break;
    case RIGHT:
      servoWheelRight.write(ENGINE_BACKWARD);  //gira a roda direita para trás
      servoWheelLeft.write(ENGINE_FORWARD);    //gira a roda esquerda para frente
      delay(500);                              //tempo necessário para efetuar a rotação
      Stop();                                  //para as rodas
      break;
    case BACKWARD:
      servoWheelRight.write(ENGINE_FORWARD);   //gira a roda direita para frente
      servoWheelLeft.write(ENGINE_BACKWARD);   //gira a roda esquerda para trás
      delay(1000);                             //tempo necessário para terminar a rotação
      Stop();                                  //para as rodas
      break;
    default:
      break;
  }
}

//Empilha um novo movimento
void PileMovement(int turn, bool canTurnLeft, bool canTurnRight, bool canGoForward) {
  Movement movement;                        //cria um movimento novo
  movement.rotation = turn;                 // rotação inicial do movimento          -
  movement.moveTime = moveTime;             // tempo de duração do movimento          |
  movement.FREE_LEFT = canTurnLeft;         // indica caminho livre para a esquerda   | --- inicializa o movimento com os parâmetros passados
  movement.FREE_RIGHT = canTurnRight;       // indica caminho livre para a direita    |
  movement.FREE_FORWARD = canGoForward;     // indica caminho livre para frente      -
  movementStack.push(movement);             //empilha o movimento na pilha de movimentos
}
