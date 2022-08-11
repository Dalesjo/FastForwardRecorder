import { Component } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent
{
  public title = 'FastForwardFrontend';
  public time: number = 0;
  public size: number = 0;
  public free: number = 0;

  /**
   * Is true while recording is ongoing.
   */
  public recording: number = 0;

  private connection: signalR.HubConnection;

  constructor()
  {
    let reconnectPolicy = new OneMinuteReconnectPolicy();
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7168/hubs/record")
      .withAutomaticReconnect(reconnectPolicy)
      .build();

    this.connection.on("State", (data: FastForwardState) =>
      this.onState(data));

    this.connection.on("Started", () =>
      this.OnStarted());

      this.connection.on("Stopped", () =>
      this.onStopped());

    this.connection.start()
      .then(() => this.onConnection());
  }

  private onConnection()
  {
    console.log("Connectione stablished");
  }

  private onState(data: FastForwardState)
  {
    console.log("State", data);

    this.time = data.time;
    this.size = data.size;
    this.free = data.freeSpace
  }

  private OnStarted()
  {
    console.log("OnStarted");
    this.recording = 1;
  }

  private onStopped()
  {
    console.log("onStopped");
    this.recording = 2;
  }

  public record()
  {
    console.log("record");
    this.time = 0;
    this.size = 0;
    this.connection.send("Start");
  }

  public stop()
  {
    console.log("stop");
    this.connection.send("Stop");
  }
}

class OneMinuteReconnectPolicy implements signalR.IRetryPolicy
{

  nextRetryDelayInMilliseconds(retryContext: signalR.RetryContext): number | null
  {

    if (retryContext.elapsedMilliseconds < 60000)
    {
      return 2000;
    }

    return 10000;
  }

};


interface FastForwardState
{
  bitrate: number;
  size: number;
  speed: number;
  time: number;
  freeSpace: number;
}
