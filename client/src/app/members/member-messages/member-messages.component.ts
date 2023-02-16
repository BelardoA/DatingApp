import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {MessageService} from "../../_services/message.service";
import {Message} from "../../_models/message";
import {NgForm} from "@angular/forms";

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm;
  @Input() userName?: string;
  @Input() messages: Message[] = [];
  messageContent = '';

  constructor(private messageService: MessageService) {
  }

  ngOnInit(): void {
  }

  sendMessage() {
    if (!this.userName) return;
    this.messageService.sendMessage(this.userName, this.messageContent).subscribe({
      next: message => {
        this.messages.push(message);
        this.messageForm?.reset();
      }
    })
  }

}
