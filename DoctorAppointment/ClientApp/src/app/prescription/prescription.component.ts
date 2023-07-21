import { Component } from '@angular/core';

@Component({
  selector: 'app-prescription-component',
  templateUrl: './prescription.component.html'
})
export class PrescriptionComponent {
  public currentCount = 0;

  public incrementCounter() {
    this.currentCount++;
  }
}

