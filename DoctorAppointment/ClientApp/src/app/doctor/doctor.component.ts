import { Component } from '@angular/core';

@Component({
  selector: 'app-doctor-component',
  templateUrl: './doctor.component.html'
})
export class DoctorComponent {
  public currentCount = 0;

  public incrementCounter() {
    this.currentCount++;
  }
}

