import { Component } from '@angular/core';
import {HttpClient} from '@angular/common/http'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  
  title = 'EnsekMeterReadings';
  fileName = '';
  numberOfFaultyRecords = 0;
  errorMessage = "";

  constructor(private http: HttpClient) {}

  onFileSelected(event: any) {

    const file:File = event.target.files[0];

    if (file) {

        this.fileName = file.name;

        const formData = new FormData();

        formData.append("Readings", file);

        const upload$ = this.http.post("https://localhost:44311/meter-reading-uploads", formData);

        upload$.subscribe(x => 
          {
            this.numberOfFaultyRecords = +x;
            this.errorMessage = "No errors";
          },
          err => 
          {
            this.errorMessage = err.error.error;
          });
    }
  }

}
