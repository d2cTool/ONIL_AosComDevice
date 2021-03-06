// UnmCpp.cpp: определяет точку входа для консольного приложения.
//

#include <cstdlib>
#include <iostream>
#include <fstream>
#include <ctime>
#include <Windows.h>
#include <chrono>
#include <thread>
#include <sstream>
#include <string.h>
#include <limits>

//using namespace std::chrono_literals;

typedef bool(*functionPointer1)(const char* serial1, const char* serial2);
typedef bool(*functionPointer2)();
typedef bool(*functionPointer3)(int plus, int minus, byte isDeviceOn);
typedef bool(*functionPointer4)(int firmware);
typedef bool(*functionPointer5)(const char* msg);

std::string generateValue();
std::string ReadXmlFile(std::string fullFileName);

bool bInited = false;
static int nLamp;

extern "C" {
	void InitDevice(const char*, const char*);
	void Init();
	void Stop();
	byte ShowDevice(byte);
	int SetMeasure(double, double, double, double, double);
	byte SetLamp(int, byte);
	void PlayMSound(int);
	void SetAngle(double);
	void SetValue(int);
	int GetFirmware();
	int UpdateFirmware(const char*);
	byte ChangeProbeState(byte);
	void Send(const char*);
	void SetDemoMode(byte, double, double, int);
	int IsDebugInfoHidden(int);

	bool OnConnectCallbackFunction(functionPointer1);
	bool OnDisconnectCallbackFunction(functionPointer2);
	bool OnStateChangedCallbackFunction(functionPointer3);
	bool OnChangedFirmwareCallbackFunction(functionPointer4);
	bool OnGetDataCallbackFunction(functionPointer5);
}

bool OnConnectCallback(const char* str1, const char* str2) {
	std::cout << "Connect: device serial: " << str1 << ", subdevice serial: " << str2 << std::endl;
	nLamp = 0;
	std::string pathToConf = "Ц4352-М1 Прибор.xml";
	std::string pathToDev = "Ц4352-М1.xml";

	//std::string pathToConf = "C:\\Users\\optic\\Desktop\\Ц4352-М1 Прибор.xml";
	//std::string pathToDev = "C:\\Users\\optic\\Desktop\\Ц4352-М1.xml";

	bInited = false;
	std::string xmlDev = ReadXmlFile(pathToDev);
	std::string xmlConf = ReadXmlFile(pathToConf);
	InitDevice(xmlDev.c_str(), xmlConf.c_str());

	//std::string command = "h1";

	//Send(command.c_str());
	//SetLamp(2, 1);
	//Send("h3");

	/*Send("h1");
	Send("h2");
	Send("h3");*/

	return true;
}

bool OnDisconnectCallback() {
	bInited = false;
	std::cout << "Disconnect" << std::endl;
	return true;
}

bool OnStateChangedCallback(int i1, int i2, byte isDeviceOn) {
	std::cout << "New state: plus: " << i1 << ", minus:" << i2 << ", isDeviceOn:" << (int)isDeviceOn << std::endl;

	//if (!bInited)
	{
		bInited = true;
		
		std::cout << "SetLamp(" << nLamp << ");" << std::endl;
		//SetLamp(++nLamp, 1);
		std::string l = "h" + std::to_string(nLamp);
		Send(l.c_str());
		nLamp++;

	}

	/*if (isDeviceOn)
		Send("h1");
	else
		Send("l1");*/

	//SetMeasure(.10, 2, 5, 10, 100);
	return true;
}

bool UpdateFirmwareCallback(int version) {
	std::cout << "Firmware version: " << version << std::endl;
	return true;
}

bool GetDataCallbackFunction(const char* msg) {
	std::cout << "Get msg: " << msg << std::endl;
	return true;
}

void InitCallback()
{
 	OnConnectCallbackFunction(&OnConnectCallback);
	OnDisconnectCallbackFunction(&OnDisconnectCallback);
	OnStateChangedCallbackFunction(&OnStateChangedCallback);
	OnChangedFirmwareCallbackFunction(&UpdateFirmwareCallback);
	OnGetDataCallbackFunction(&GetDataCallbackFunction);
}

int main()
{
	std::srand((int)std::time(0));

	//std::cout << "Register callbacks" << std::endl;
	InitCallback();

	//std::cout << "Init device" << std::endl;
	Init();

	std::string input;
	std::string l;
	int lla;
	int x;
	byte bb;
	std::string hexFile3 = "D:\\123123.hex";
	std::string hexFile4 = "D:\\180428.hex";

	while (true)
	{
		getline(std::cin, input);
		x = std::stoi(input);

		switch (x)
		{
		case 1:
			std::cout << "Select lamp: " << std::endl;
			getline(std::cin, l);
			lla = std::stoi(l);
			SetLamp(lla, 1);
			break;
		case 2:
			std::cout << "Firmware: " << GetFirmware() << std::endl;
			break;
		case 3:
			UpdateFirmware(hexFile3.c_str());
			break;
		case 4:
			std::cout << "Change debug info visiability: " << std::endl;
			getline(std::cin, l);
			lla = std::stoi(l);
			IsDebugInfoHidden(lla);
			break;
		case 5:
			//std::cout << "Set probe state: " << std::endl;
			//getline(std::cin, l);
			//bb = (byte)std::stoi(l);
			ChangeProbeState(1);
			break;
		case 6:
			std::cout << "Enter command: ";
			getline(std::cin, l);
			Send(l.c_str());
			break;
		case 7:
			//SetDemoMode(1, 0, 0, 0);
			//Sleep(1000);
			SetDemoMode(1, 0, 0, 2);
			Sleep(1000);
			SetDemoMode(1, 0, 270, 2);
			Sleep(1000);
			SetDemoMode(1, 36, 270, 2);
			Sleep(1000);
			SetDemoMode(1, 0, 270, 2);
			break;
		case 8:
			SetDemoMode(1, 115.8, 203.7, 2);
			break;
		case 9:
			SetDemoMode(0, 115.8, 203.7, 2);
			break;
		case 11:
			SetDemoMode(1, 0, 0, 2);
		case 12:
			SetDemoMode(1, 0, 270, 2);
		case 13:
			SetDemoMode(1, 36, 270, 2);
		case 14:
			SetDemoMode(1, 0, 270, 2);
			break;
		default:
			break;
		}
	}

	return EXIT_SUCCESS;
}

std::string generateValue() {
	int random_variable = std::rand() & 0xFF;
	std::stringstream strs;
	std::string command = "v";
	strs << command << random_variable;
	std::string temp_str = strs.str();
	return temp_str;
}

std::string ReadXmlFile(std::string fullFileName) {
	std::ifstream ifs(fullFileName);
	std::string content((std::istreambuf_iterator<char>(ifs)),
		(std::istreambuf_iterator<char>()));

	return content;
}


