const url = require("url");
const path = require("path");
const { ConnectionBuilder } = require("electron-cgi");

import { app, BrowserWindow } from "electron";

// Keep a global reference of the window object, if you don't, the window will
// be closed automatically when the JavaScript object is garbage collected.
let window: BrowserWindow | null;
let _connection: null;

const createWindow = () => {
  // Create the browser window.
  window = new BrowserWindow({
      width: 1600,
      height: 1024,
      webPreferences: {
          nodeIntegration: true
        }
    });

  // and load the index.html of the app.
  window.loadURL(
    url.format({
      pathname: path.join(__dirname, "index.html"),
      protocol: "file:",
      slashes: true
    })
  );

  // Open the DevTools.
  // mainWindow.webContents.openDevTools()

  // Emitted when the window is closed.
  window.on("closed", () => {
    // Dereference the window object, usually you would store windows
    // in an array if your app supports multi windows, this is the time
    // when you should delete the corresponding element.
    window = null;
  });
};

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.on("ready", createWindow);

app.on("window-all-closed", () => {
  // On macOS it is common for applications and their menu bar
  // to stay active until the user quits explicitly with Cmd + Q
  if (process.platform !== "darwin") {
    app.quit();
  }
});

app.on("activate", () => {
  // On macOS it's common to re-create a window in the app when the
  // dock icon is clicked and there are no other windows open.
  if (window === null) {
    createWindow();
  }
});

function setupConnectionToRestartOnConnectionLost() {
    _connection = new ConnectionBuilder()
        .connectTo("dotnet", "run", "--project", "./dotnet/DatafyApp")
        .build();

    _connection.onDisconnect = () => {
        //alert('Connection lost, restarting...');
        console.log("lost");
        setupConnectionToRestartOnConnectionLost();
    };
    
    _connection.send("greeting", "Mom from C#", (error: any, response: any) => {
        console.log(response);
        window.webContents.send("greeting", response);
        //_connection.close();
      });

    _connection.send("gettypenames", (error: any, response: any) => {
        console.log(response);
      });
}

setupConnectionToRestartOnConnectionLost();