import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-NameStep',
  templateUrl: './NameStep.component.html',
  styleUrls: ['./NameStep.component.css'],
  imports: [FormsModule],
})
export class NameStepComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
  @Output() next = new EventEmitter();
  name = '';

  nextStep() {
    if (this.name.trim()) {
      this.next.emit({ name: this.name });
    }
  }
}
