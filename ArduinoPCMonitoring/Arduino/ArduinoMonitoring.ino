#define SCREEN_WIDTH 128
#define SCREEN_HEIGHT 32

#include <Keypad.h>
#include <Adafruit_Fingerprint.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <Wire.h>
#include <SPI.h>

byte isWaitInput = 0;
byte isOpen = 0;

const byte rows = 4;
const byte cols = 4;

byte rowPins[rows] = { 7, 6, 5, 4 };
byte colPins[cols] = { 11, 10, 9, 8 };

char keys[rows][cols] = 
{
    { '1', '2', '3', 'A' },
    { '4', '5', '6', 'B' },
    { '7', '8', '9', 'C' },
    { '*', '0', '#', 'D' }
};

char pwCount = 0;

char password[10] = { 0 };
char tPassword[10] = { 0 };

Keypad dKeypad = Keypad(makeKeymap(keys), rowPins, colPins, rows, cols);
SoftwareSerial fingerSerial(2, 3);
SoftwareSerial btSerial(12, 13);
Adafruit_Fingerprint fingerSensor = Adafruit_Fingerprint(&fingerSerial);

Adafruit_SSD1306 display = Adafruit_SSD1306(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, -1); 

void setup()
{
    //pinMode(LED_BUILTIN, OUTPUT);
    //digitalWrite(LED_BUILTIN, LOW);
  
    Serial.begin(9600);
    btSerial.begin(9600);

    if (!display.begin(SSD1306_SWITCHCAPVCC, 0x3C))
    {
        while (1);
    }

    initDisplay();

    fingerSensor.begin(57600);
    fingerSensor.emptyDatabase();

    welcome();
    firstSetting();
}

void loop()
{
    btSerial.listen();

    if (Serial.available())
    {
        char data = Serial.read();

        if (data == '0')
        {
            setStringDisplay("Door C");
            isOpen = 0;
            delay(1000);
        }
        else if (data == '1')
        {
            setStringDisplay("Door O");
            isOpen = 1;
            delay(1000);
        }
    }
    else if (!isOpen)
    {
        showWaitInput();

        char key = dKeypad.getKey();

        btSerial.listen();
        
        if (key != NO_KEY)
        {
            if ((pwCount < 10) && (key >= '0') && (key <= '9'))
            {
                tPassword[pwCount++] = key;
            }
            else if (key == '*')
            {
                if (checkKeypadPassword())
                {
                    sendOpenSignal();
                }
            }
            else if (key == '#')
            {
                resetTempPassword();
            }
            else if (key == 'A')
            {
                setStringDisplay("Change PW");
                delay(1000);
                changeKeypadPassword();
            }
            else if (key == 'B')
            {
                fingerSerial.listen();
              
                setStringDisplay("Add FP");
                delay(1000);
                resetTempPassword();
                setStringDisplay("Check PW");

                while (1)
                {
                    key = dKeypad.getKey();

                    if (key != NO_KEY)
                    {
                        if ((pwCount < 10) && (key >= '0') && (key <= '9'))
                        {
                            tPassword[pwCount++] = key;
                            showPasswordDisplay();
                        }
                        else if (key == '*')
                        {
                            if (checkKeypadPassword())
                            {
                                setFingerPrint();
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else if (pwCount >= 10)
                    {
                        if (checkKeypadPassword())
                            {
                                setFingerPrint();
                                break;
                            }
                            else
                            {
                                break;
                            }
                    }
                }
            }
            else if (key == 'C')
            {
                fingerSerial.listen();
              
                setStringDisplay("Del FP?");
                delay(2000);
                display.println("");
                display.println("1 : Y, 2: N");

                display.display();

                while ((key = dKeypad.getKey()) == NO_KEY);

                if (key == '1')
                {
                    fingerSensor.emptyDatabase();
                    setStringDisplay("Del O");
                    delay(2000);
                }
            }
            else if (key == 'D')
            {
                sendCloseSignal();
            }
        }
        else if (pwCount >= 10)
        {
            if (checkKeypadPassword())
            {
                sendOpenSignal();
            }
        }
        else
        {
            delay(100);
          
            if (btSerial.available())
            {
                resetTempPassword();

                while (btSerial.available())
                {
                    char data = btSerial.read();

                    if ((data >= '0') && (data <= '9'))
                    {
                        tPassword[pwCount++] = data;
                    }
                    else if (data == '*')
                    {
                        break;
                    }
                }

                if (checkKeypadPassword())
                {
                    sendOpenSignal();
                }
            }

            fingerSerial.listen();

            if (checkFingerSensorStatus() && (fingerSensor.getImage() != FINGERPRINT_NOFINGER))
            {
                checkFingerPrint();
            }

            btSerial.listen();
        }
        
        //delay(100);
    }
}

void sendOpenSignal()
{
    Serial.write('1');
}

void sendCloseSignal()
{
    Serial.write('0');
}

void sendFaultSignal()
{
    Serial.write('2');
}

void welcome()
{
    display.clearDisplay();

    display.setCursor(0, 0);
    display.println("Welcome!\n");
    display.println("* SDL System *");

    display.display();

    delay(3000);
}

void firstSetting()
{
    char key;

    setKeypadPassword();
    
    display.clearDisplay();

    display.setCursor(0, 0);
    display.println("Use FP?");
    display.println("");
    display.println("1 : Y, 2: N");

    display.display();

    while ((key = dKeypad.getKey()) == NO_KEY);

    if (key == '1')
    {
        setFingerPrint();
    }

    display.clearDisplay();

    display.setCursor(0, 0);
    display.println("First Set O");

    display.display();

    delay(2000);
}

void showWaitInput()
{
    display.clearDisplay();

    display.setCursor(1, 0);
    display.println("Wait...");
    display.println("");

    for (char i = 0; i < pwCount; ++i)
    {
        display.print("*");
    }

    display.display();

    isWaitInput = 1;
}

void showPasswordDisplay()
{
    display.clearDisplay();

    display.setCursor(0, 0);
    display.println("Input PW");
    display.println("");

    for (char i = 0; i < pwCount; ++i)
    {
        display.print("*");
    }

    display.display();
}

void resetTempPassword()
{
    for (char i = 0; i < 10; ++i)
    {
        tPassword[i] = 0;
    }

    pwCount = 0;
}

void setKeypadPassword()
{
    char key;

    setStringDisplay("Set PW");

    while (1)
    {
        key = dKeypad.getKey();

        if (key != NO_KEY)
        {
            if ((pwCount < 10) && (key >= '0') && (key <= '9'))
            {
                password[pwCount++] = key;
                showPasswordDisplay();
            }
            else if (key == '*')
            {
                if (pwCount >= 1)
                {
                    setStringDisplay("Set PW O");
                    delay(2000);
                    break;
                }
                else
                {
                    setStringDisplay("Empty");
                    delay(2000);
                    showPasswordDisplay();
                }
            }
        }
        else if (pwCount >= 10)
        {
            setStringDisplay("Set PW O");
            delay(2000);
            break;
        }
    }

    pwCount = 0;
}

void changeKeypadPassword()
{
    char key;
    char count = 0;

    resetTempPassword();

    setStringDisplay("Input PW");

    while (1)
    {
        key = dKeypad.getKey();

        if (count >= 5)
        {
            return;
        }

        if (key != NO_KEY)
        {
            if ((pwCount < 10) && (key >= '0') && (key <= '9'))
            {
                tPassword[pwCount++] = key;
                showPasswordDisplay();
            }

            if (key == '*')
            {
                if (checkKeypadPassword())
                {
                    setStringDisplay("PW O");
                    delay(2000);
                    break;
                }
                else
                {
                    setStringDisplay("Try again");
                    delay(2000);
                    resetTempPassword();
                    count += 1;
                } 
            }
        }
        else if (pwCount >= 10)
        {
            if (checkKeypadPassword())
            {
                setStringDisplay("PW O");
                delay(2000);
                break;
            }
            else
            {
                setStringDisplay("Try again");
                delay(2000);
                resetTempPassword();
            } 
        }
    }

    resetTempPassword();
    setKeypadPassword();
}

boolean checkKeypadPassword()
{
    byte isCorrect = 1;

    for (char i = 0; i < 10; ++i)
    {
        if (password[i] != tPassword[i])
        {
            isCorrect = 0;
            break;
        }
    }

    if (isCorrect)
    {
        setStringDisplay("PW O");
        delay(2000);
    }
    else
    {
        setStringDisplay("PW X");
        delay(2000);
    }

    pwCount = 0;
    resetTempPassword();

    return (isCorrect == 1);
}

void setFingerPrint()
{
    if (!checkFingerSensorStatus())
    {
        setStringDisplay("FP not working");
        delay(2000);
        showWaitInput();
        return;
    }

    fingerSensor.getTemplateCount();

    if (enrollFingerPrint(fingerSensor.templateCount + 1))
    {
        setStringDisplay("FP set O");
    }
    else
    {
        setStringDisplay("FP set X");
    }

    delay(2000);
}

void checkFingerPrint()
{
    if (verifyFingerPrint())
    {
        setStringDisplay("FP O");
        sendOpenSignal();
        isOpen = 1;
    }
    else
    {
        setStringDisplay("FP X");
        delay(2000);
    }
}

boolean checkFingerSensorStatus()
{
    return fingerSensor.verifyPassword();
}

boolean enrollFingerPrint(uint8_t id)
{
    int p = -1;
    char count = 0;

CHECK:
    p = -1;
    count = 0;

    setStringDisplay("Waiting...");

    while ((p = fingerSensor.getImage()) != FINGERPRINT_OK) 
    {
        switch (p) 
        {
            case FINGERPRINT_OK:
                display.print("S");
                display.display();
                break;
            case FINGERPRINT_NOFINGER:
                if (!(count % 10))
                {
                    display.print(".");
                    display.display();
                }
                break;
            case FINGERPRINT_PACKETRECIEVEERR:
            case FINGERPRINT_IMAGEFAIL:
            default:
                display.println("F");
                display.display();
                delay(1000);
                goto CHECK;
                break;
        }

        delay(100);
        count += 1;

        if (count >= 100)
        {
            return false;
        }
    }

    delay(1000);

    setStringDisplay("Convert...");
    p = fingerSensor.image2Tz(1);

    switch (p) 
    {
        case FINGERPRINT_OK:
            display.print("S");
            display.display();
            break;
        case FINGERPRINT_IMAGEMESS:
        case FINGERPRINT_PACKETRECIEVEERR:
        case FINGERPRINT_FEATUREFAIL:
        case FINGERPRINT_INVALIDIMAGE:
        default:
            display.print("F");
            display.display();
            delay(1000);
            return false;
    }

    delay(1000);

    /*setStringDisplay("Remove finger");
    p = 0;

    delay(1000);

    while ((p = fingerSensor.getImage()) != FINGERPRINT_NOFINGER);*/

    waitRemoveFinger();

CHECK2:

    p = -1;
    count = 0;

    setStringDisplay("Place same finger");

    while ((p = fingerSensor.getImage()) != FINGERPRINT_OK) 
    {
        switch (p) 
        {
            case FINGERPRINT_OK:
                display.print("S");
                display.display();
                break;
            case FINGERPRINT_NOFINGER:
                if (!(count % 10))
                {
                    display.print(".");
                    display.display();
                }
                break;
            case FINGERPRINT_PACKETRECIEVEERR:
            case FINGERPRINT_IMAGEFAIL:
            default:
                display.print("F");
                display.display();
                delay(1000);
                goto CHECK2;
                break;
        }

        delay(100);
        count += 1;

        if (count >= 100)
        {
            return false;
        }

    }

    delay(1000);

    setStringDisplay("Convert...");
    p = fingerSensor.image2Tz(2);

    switch (p) 
    {
        case FINGERPRINT_OK:
            display.print("S");
            display.display();
            break;
        case FINGERPRINT_IMAGEMESS:
        case FINGERPRINT_PACKETRECIEVEERR:
        case FINGERPRINT_FEATUREFAIL:
        case FINGERPRINT_INVALIDIMAGE:
        default:
            display.print("F");
            display.display();
            delay(1000);
            return false;
    }

    delay(500);

    setStringDisplay("Save...");  
    display.print("ID No. ");
    display.println(id);
    
    display.display();

    p = fingerSensor.createModel();

    switch (p)
    {
        case FINGERPRINT_OK:
            break;
        case FINGERPRINT_PACKETRECIEVEERR:
        case FINGERPRINT_ENROLLMISMATCH:
        default:
            display.print("F");
            display.display();
            delay(1000);
            return false;
    }

    p = fingerSensor.storeModel(id);

    switch (p)
    {
        case FINGERPRINT_OK:
            display.print("S");
            display.display();
            break;
        case FINGERPRINT_PACKETRECIEVEERR:
        case FINGERPRINT_BADLOCATION:
        case FINGERPRINT_FLASHERR:
        default:
            display.print("F");
            display.display();
            delay(1000);
            return false;
    }

    delay(1000);

    /*setStringDisplay("Remove finger");
    p = 0;

    delay(1000);

    while ((p = fingerSensor.getImage()) != FINGERPRINT_NOFINGER);*/

    waitRemoveFinger();

    return true;
}

void waitRemoveFinger()
{
    int p = 0;
  
    setStringDisplay("Remove finger");

    while ((p = fingerSensor.getImage()) != FINGERPRINT_NOFINGER);

    delay(1000);
}

boolean verifyFingerPrint()
{
    uint8_t p = fingerSensor.getImage();

    if (p != FINGERPRINT_OK)
    {
        return false;
    }

    p = fingerSensor.image2Tz();

    if (p != FINGERPRINT_OK)  
    {
        return false;
    }

    p = fingerSensor.fingerFastSearch();

    if (p != FINGERPRINT_OK)  
    {
        return false;
    }
    
    return true; 
}

void initDisplay()
{
    display.clearDisplay();
    display.setTextSize(1);
    display.setTextColor(SSD1306_WHITE);
}

void setStringDisplay(String s)
{
    display.clearDisplay();

    display.setCursor(0, 0);
    display.println(s);

    display.display(); 
}
