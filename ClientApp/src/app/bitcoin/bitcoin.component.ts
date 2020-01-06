import { Component, OnInit, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { Message } from '@angular/compiler/src/i18n/i18n_ast';

@Component({
  selector: 'app-bitcoin-component',
  templateUrl: './bitcoin.component.html',
  styleUrls: ['./bitcoin.component.css']
})

export class BitcoinComponent implements OnInit, OnDestroy {
    bitcoinNodeAction: string;
    monitoring: Monitoring;

    private hubConnection: HubConnection;

    ngOnInit(): void {
        this.bitcoinNodeAction = "Run node";

        this.monitoring = {
            currentTime: "...",
            bitcoinNodeStatus: "Stopped",
            bitcoinBlockCount: 0
        };

        this.hubConnection = new HubConnectionBuilder().withUrl("/monitoringhub").build();

        this.hubConnection.on("UpdateTime", (message) => {
            this.monitoring.currentTime = message;
        });

        this.hubConnection.on("UpdateBitcoinNodeState", (message) => {
            this.monitoring.bitcoinNodeStatus = message;
        });

        this.hubConnection.on("UpdateBitcoinBlockCount", (message) => {
            this.monitoring.bitcoinBlockCount = message;
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

    onChangeNodeState() {
      if (this.monitoring.bitcoinNodeStatus == "Running") {
        this.hubConnection.invoke("DownBItcoinNode");
        this.bitcoinNodeAction = "Run node";
      }
      else if (this.monitoring.bitcoinNodeStatus == "Stopped") {
        this.hubConnection.invoke("RunBitcoinNode");
        this.bitcoinNodeAction = "Down node";
      }
    }
}

interface Monitoring {
    currentTime: string;
    bitcoinBlockCount: number;
    bitcoinNodeStatus: string;
}
