import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BadgeCheck, LucideAngularModule } from 'lucide-angular';
import { CheckCircle, XCircle, Info, AlertTriangle } from 'lucide-angular';
@Component({
  selector: 'app-toastr',
  templateUrl: './toastr.component.html',
  styleUrls: ['./toastr.component.css'],
  imports: [CommonModule, LucideAngularModule, FormsModule],
})
export class ToastrComponent implements OnInit {
  icon!: any;
  constructor() {}
  @Input() message = '';
  @Input() type: 'success' | 'error' | 'info' = 'success';
  show = true;

  ngOnInit() {
    this.setIcon();
    this.show = true;
    setTimeout(() => (this.show = false), 3000);
  }
  setIcon() {
    switch (this.type) {
      case 'success':
        this.icon = BadgeCheck;
        break;
      case 'error':
        this.icon = XCircle;
        break;

      default:
        this.icon = Info;
    }
  }
}
