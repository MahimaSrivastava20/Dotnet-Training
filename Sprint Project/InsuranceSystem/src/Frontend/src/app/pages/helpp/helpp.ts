import { Component } from '@angular/core';

@Component({
  selector: 'app-helpp',
  imports: [],
  templateUrl: './helpp.html',
  styleUrl: './helpp.css',
})
export class Helpp {

  onChange(event:Event) {
    const input = event.target as HTMLInputElement;
    alert(input.value);
  }
}
