import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'bindig-test';
  pathImages = '/assets/images/';
  images = ["1.jpg", "2.jpg", "3.jpg"];
  selectedImage = this.pathImages + this.images[0];
  random = 0;

  changeImage() {
    this.random = Math.floor((Math.random() * 3) + 1);
    this.selectedImage = this.pathImages + this.images[this.random];
  }
}
