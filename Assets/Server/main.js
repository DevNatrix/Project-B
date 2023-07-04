const dgram = require('dgram');
const { join } = require('path');
const { send, cpuUsage } = require('process');
const server = dgram.createSocket('udp4');
const fs = require('fs');
const { randomInt } = require('crypto');
const { get } = require('http');

const validCommands = ['u', 'e', 'newClient', 'leave']; // u = update, e = event (short for conservation of bandwidth)

currentID = 0; //the ID given to players when they join
TPS = 32; //Ticks per second - this is how fast you want players to update their position and check for events
SERVERPORT = 12345;

const maxChecksBeforeDisconnect = 3; //this times diconnect interval is how long it takes (in ms) for a player to get disconnected
setInterval(checkDisconnectTimers, 1000);

playerTransformInfo = []; //position and rotation
playerInfo = []; //usernames, might be more later
currentPlayerIDs = []; //IDs (to find where other information is without having to do larger calculations)
playerDisconnectTimers = []; //disconnect timers that go up untill they are disconnected because of not updating their transform
eventsToSend = []; //events that que up untill the client calls an update, where they are dumped and the client then processes them

//if server runs into an un-handled error
server.on('error', (err) => {
	console.log(`server error:\n${err.stack}`);
	server.close();
});

//on server start
server.on('listening', () => {
	const address = server.address();
	console.log(`server listening on ${address.address}:${address.port}`);
});


//on message recieved
server.on('message', (msg, senderInfo) => {
	msg = msg + "";
	console.log(msg);
	try {
		if (validCommands.includes(msg.split("~")[0])) {
			eval(msg.split("~")[0] + "(\"" + msg + "\", " + senderInfo.port + ", \"" + senderInfo.address + "\")");
		}
		else {
			console.warn("Unknown command (prob from a dumb bot, but maybe not)");
			logSenderInfo(msg, senderInfo);
			console.log("---------------------");
		}
	} catch (error) {
		console.error(error);
		logSenderInfo(msg, senderInfo);
		console.log("---------------------");
	}
});


//Server functions -----------------------------------------------------------------------------
function checkDisconnectTimers() {
	/*console.log("_____________")
	console.log("Transforms: ");
	console.log(playerTransformInfo);
	console.log("Current player IDs: ");
	console.log(currentPlayerIDs);
	console.log("Player disconnect timers: ");       //this basically debugs everything in one second intervals
	console.log(playerDisconnectTimers);
	console.log("Player usernames: ");
	console.log(playerInfo);
	console.log("Events to send: ");
	console.log(eventsToSend);*/

	//find all players to disconnect
	playerIndexesToDisconnect = [];
	for (playerListID in playerDisconnectTimers) {
		playerDisconnectTimers[playerListID]++;
		if (playerDisconnectTimers[playerListID] > maxChecksBeforeDisconnect) {
			playerIndexesToDisconnect.push(playerListID);
		}
	}

	//disconnect all of the players that timed out
	if (playerIndexesToDisconnect.length >= 1) {
		console.log("Player TimedOut: " + playerIndexesToDisconnect);
		for (playerIndexesID in playerIndexesToDisconnect) {
			disconnectClient(playerIndexesToDisconnect[playerIndexesToDisconnect.length - 1 - playerIndexesID]);
		}
	}
}

function disconnectClient(playerIndex) {
	addEventToAll("removeClient~" + currentPlayerIDs[playerIndex] + "|");
	delete currentPlayerIDs[playerIndex];
	delete playerDisconnectTimers[playerIndex];
	delete playerTransformInfo[playerIndex];
	delete playerInfo[playerIndex];
	delete eventsToSend[playerIndex];
}

function addEventToAll(eventString) {
	for (eventsToSendID in eventsToSend) {
		eventsToSend[eventsToSendID] += eventString + "|";
	}
}

//for debuging a message
function logSenderInfo(msg, senderInfo) {
	console.log("---------------------");
	console.log("Date/Time: " + Date());
	console.log("Message: " + msg);
	console.log("Port: " + senderInfo.port);
	console.log("Address: " + senderInfo.address);
}

//Client functions -----------------------------------------------------------------------------
function leave(info, senderPort, senderAddress) {
	disconnectClient(currentPlayerIDs.indexOf(parseInt(info.split("~")[1])));
	console.log("Player with ID " + info.split("~")[1] + " has left the game");
}

function newClient(info, senderPort, senderAddress) {
	server.send(currentID + "~" + TPS, senderPort, senderAddress);

	splitInfo = info.split("~");

	addEventToAll("newClient~" + currentID + "~" + splitInfo[1]);
	console.log("---------------------");
	console.log("Date/Time: " + Date());
	console.log("New client: ID = " + currentID + ", Username = " + splitInfo[1]);
	console.log("---------------------");

	allPlayerJoinInfo = "";
	for (playerIndex in currentPlayerIDs) {
		allPlayerJoinInfo += "newClient~" + currentPlayerIDs[playerIndex] + "~" + playerInfo[playerIndex] + "|";
	}
	eventsToSend.push(allPlayerJoinInfo);
	playerInfo.push(splitInfo[1]);
	playerTransformInfo.push("(0, 0, 0)~(0, 0, 0, 1)");
	playerDisconnectTimers.push(0);
	currentPlayerIDs.push(currentID);

	currentID++;
}

function e(info, senderPort, senderAddress) {
	splitInfo = info.split("~");
	newEvent = info.substring(splitInfo[0].length + 1, info.length);
	console.log("added event: " + newEvent);
	addEventToAll(newEvent);
}

async function u(info, senderPort, senderAddress) {
	splitInfo = info.split("~")
	console.log("Player with ID " + splitInfo[1] + " updated");
	transformsToSend = "";
	for (playerIndex in currentPlayerIDs) {
		if (currentPlayerIDs[playerIndex] != splitInfo[1]) {
			transformsToSend += "u~" + currentPlayerIDs[playerIndex] + "~" + playerTransformInfo[playerIndex] + "|"
		}
	}
	playerIndex = currentPlayerIDs.indexOf(parseInt(splitInfo[1]));
	if (playerIndex != -1) {
		server.send(eventsToSend[playerIndex] + transformsToSend, senderPort, senderAddress); //send events, and later other players new positions
		eventsToSend[playerIndex] = "";
		playerTransformInfo[playerIndex] = splitInfo[2] + "~" + splitInfo[3];
		playerDisconnectTimers[playerIndex] = 0;
	}
	else {
		console.log("ERROR: player with ID " + splitInfo[1] + " is not currently in the game but tried to update transform");
	}
}


//quick tools
function sleep(milliseconds) {
	const date = Date.now();
	let currentDate = null;
	do {
		currentDate = Date.now();
	} while (currentDate - date < milliseconds);
}
function getRandomInt(max) {
	return Math.floor(Math.random() * max);
}

server.bind(SERVERPORT);
