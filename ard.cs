// Defining LED's and Buttons

#define p1Button 3
#define p2Button 4
#define p3Button 5
#define hostButton 6
#define p1Led 8
#define p2Led 9
#define p3Led 10
#define hostLed 11
#define speaker 13

#define NOTE_B0  31
#define NOTE_C1  33
#define NOTE_CS1 35
#define NOTE_D1  37
#define NOTE_DS1 39
#define NOTE_E1  41
#define NOTE_F1  44
#define NOTE_FS1 46
#define NOTE_G1  49
#define NOTE_GS1 52
#define NOTE_A1  55
#define NOTE_AS1 58
#define NOTE_B1  62
#define NOTE_C2  65
#define NOTE_CS2 69
#define NOTE_D2  73
#define NOTE_DS2 78
#define NOTE_E2  82
#define NOTE_F2  87
#define NOTE_FS2 93
#define NOTE_G2  98
#define NOTE_GS2 104
#define NOTE_A2  110
#define NOTE_AS2 117
#define NOTE_B2  123
#define NOTE_C3  131
#define NOTE_CS3 139
#define NOTE_D3  147
#define NOTE_DS3 156
#define NOTE_E3  165
#define NOTE_F3  175
#define NOTE_FS3 185
#define NOTE_G3  196
#define NOTE_GS3 208
#define NOTE_A3  220
#define NOTE_AS3 233
#define NOTE_B3  247
#define NOTE_C4  262
#define NOTE_CS4 277
#define NOTE_D4  294
#define NOTE_DS4 311
#define NOTE_E4  330
#define NOTE_F4  349
#define NOTE_FS4 370
#define NOTE_G4  392
#define NOTE_GS4 415
#define NOTE_A4  440
#define NOTE_AS4 466
#define NOTE_B4  494
#define NOTE_C5  523
#define NOTE_CS5 554
#define NOTE_D5  587
#define NOTE_DS5 622
#define NOTE_E5  659
#define NOTE_F5  698
#define NOTE_FS5 740
#define NOTE_G5  784
#define NOTE_GS5 831
#define NOTE_A5  880
#define NOTE_AS5 932
#define NOTE_B5  988
#define NOTE_C6  1047
#define NOTE_CS6 1109
#define NOTE_D6  1175
#define NOTE_DS6 1245
#define NOTE_E6  1319
#define NOTE_F6  1397
#define NOTE_FS6 1480
#define NOTE_G6  1568
#define NOTE_GS6 1661
#define NOTE_A6  1760
#define NOTE_AS6 1865
#define NOTE_B6  1976
#define NOTE_C7  2093
#define NOTE_CS7 2217
#define NOTE_D7  2349
#define NOTE_DS7 2489
#define NOTE_E7  2637
#define NOTE_F7  2794
#define NOTE_FS7 2960
#define NOTE_G7  3136
#define NOTE_GS7 3322
#define NOTE_A7  3520
#define NOTE_AS7 3729
#define NOTE_B7  3951
#define NOTE_C8  4186
#define NOTE_CS8 4435
#define NOTE_D8  4699
#define NOTE_DS8 4978

// Setup button states
int player1State = 0;
int player2State = 0;
int player3State = 0;
int hostState = 0;

int gameStarted = 0;
int counter1 = 0;
int counter2 = 0;

// Melody sequence for the jeopardy tune
int tuneMelody[] = {
  NOTE_C5, NOTE_F5, NOTE_C5, NOTE_F4,
  NOTE_C5, NOTE_F5, NOTE_C5, 0,
  NOTE_C5, NOTE_F5, NOTE_C5, NOTE_F5,
  NOTE_A5, NOTE_G5, NOTE_F5, NOTE_E5, NOTE_D5, NOTE_CS5, NOTE_C5, NOTE_F5, NOTE_C5, NOTE_F4,
  NOTE_C5, NOTE_F5, NOTE_C5, 0, NOTE_F5, NOTE_D5, NOTE_C5, NOTE_AS4, NOTE_A4, NOTE_G4, NOTE_F4, 0
};

// Duration for each note in the melody
unsigned long int noteDurations[] = {
  500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 750, 250, 250, 250, 250, 250,
  500, 500, 500, 500, 500, 500, 500, 500, 750, 250, 500, 500, 500, 500, 500, 1500
};

// Current note index (for both noteDurations and tuneMelody)
int currentNoteIndex = 0;

// Time when current note started playing
unsigned long noteStartTime = 0;

// Records when button is pressed to turn off after a certain duration
unsigned long buttonPressStart = 0;

void setup() {
  // Set up LEDs, buttons, and speaker
  pinMode(speaker, OUTPUT);
  pinMode(player1Button, INPUT);
  pinMode(player2Button, INPUT);
  pinMode(player3Button, INPUT);
  pinMode(hostButton, INPUT);
  pinMode(player1LED, OUTPUT);
  pinMode(player2LED, OUTPUT);
  pinMode(player3LED, OUTPUT);
  pinMode(hostLED, OUTPUT);

  // Turn on host LED to indicate the game has begun and program is running
  digitalWrite(hostLED, HIGH);

  // Start the serial monitor for debugging
  Serial.begin(9600);
}

void loop() {
  playMusic();
  readButtonStates();
  player1Logic();
  player2Logic();
  player3Logic();
  hostLogic();
  stopMusic();
}

void playMusic() {
  // Check if it's time to move to the next note
  if (gameStarted == 1) {
    if (millis() - noteStartTime > noteDurations[currentNoteIndex]) {
      // Stop the tone playing on the buzzer
      noTone(speaker);
      // Play the next note on the buzzer
      tone(speaker, tuneMelody[currentNoteIndex], noteDurations[currentNoteIndex]);
      // Update the note start time
      noteStartTime = millis();
      // Move to the next note in the melody and noteDurations (loop back to the start if at the end)
      currentNoteIndex = (currentNoteIndex + 1) % (sizeof(tuneMelody) / sizeof(int));
    }
  }
}

void readButtonStates() {
  player1State = digitalRead(p1Button);
  player2State = digitalRead(p2Button);
  player3State = digitalRead(p3Button);
  hostState = digitalRead(hostButton);
}

void resetGame() {
  // Play buzzer tone after the time limit
  tone(speaker, 2000, 3000);
  // Turn off all player LEDs and turn the host LED back on
  digitalWrite(p1Led, LOW);
  digitalWrite(p2Led, LOW);
  digitalWrite(p3Led, LOW);
  digitalWrite(hostLed, HIGH);
  // Reset counters and indices
  counter1 = 0;
  counter2 = 0;
  currentNoteIndex = 0;
  gameStarted = 0;
  noTone(speaker);
}

void player1Logic() {
  // If the button is pressed and no button has already been pressed
  if (player1State == HIGH && counter1 == 0) {
    // Start the game and turn on player 1 LED, turn off host LED
    gameStarted = 1;
    digitalWrite(p1Led, HIGH);
    digitalWrite(hostLed, LOW);
    buttonPressStart = millis();
    counter2 = 1;
    counter1 = 1;
  }
}

void player2Logic() {
  // If the button is pressed and no button has already been pressed
  if (player2State == HIGH && counter1 == 0) {
    // Start the game and turn on player 2 LED, turn off host LED
    gameStarted = 1;
    digitalWrite(p2Led, HIGH);
    digitalWrite(hostLed, LOW);
    buttonPressStart = millis();
    counter2 = 1;
    counter1 = 1;
  }
}

void player3Logic() {
  // If the button is pressed and no button has already been pressed
  if (player3State == HIGH && counter1 == 0) {
    // Start the game and turn on player 3 LED, turn off host LED
    gameStarted = 1;
    digitalWrite(p3Led, HIGH);
    digitalWrite(hostLed, LOW);
    buttonPressStart = millis();
    counter2 = 1;
    counter1 = 1;
  }
}

void hostLogic() {
  if (hostState == HIGH && counter1 == 1) {
    Serial.println("Before reset");
    digitalWrite(hostLed, HIGH);
    resetGame();
  }
}

void stopMusic() {
  if (millis() - buttonPressStart > 15000UL && counter2 == 1) {
    Serial.println("Reset");
    resetGame();
  }
}
