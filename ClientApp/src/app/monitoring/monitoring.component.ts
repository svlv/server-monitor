import { Component, OnInit, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

@Component({
  selector: 'app-monitoring-component',
  templateUrl: './monitoring.component.html',
  styleUrls: ['./monitoring.component.css']
})

export class MonitoringComponent implements OnInit, OnDestroy {
    monitoring: Monitoring;

    private hubConnection: HubConnection;

    ngOnInit(): void {
        this.monitoring = {
            currentTime: "..."
        };

        this.hubConnection = new HubConnectionBuilder().withUrl("/monitoringhub").build();

        this.hubConnection.on("UpdateTime", (message) => {
            this.monitoring.currentTime = message;
        });

        this.hubConnection.start().then(function () {
            console.log("Hub connection has started.");
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }

    ngOnDestroy(): void {
        console.log("Hub connection has been stoped.");
        this.hubConnection.stop();
    }
}

interface Monitoring {
    currentTime: string;
}
