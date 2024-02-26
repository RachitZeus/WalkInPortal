import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { IJobData, IApplicationInfo } from './JobData';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

import { DataService } from 'src/app/shared/data.service';
import { ApplicationService } from 'src/app/shared/application.service';
import { IJob } from 'src/interface/interfaces';
import { baseurl } from '../shared/url';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-job-page',
  templateUrl: './job-page.component.html',
  styleUrls: ['./SCSS/style.scss'],
})
export class JobPageComponent implements OnInit {

  Job: IJob[] = [];
  applicationArray: IApplicationInfo[] = [];
  remainingDays: number = 99;
  jwtHelperService= new JwtHelperService();
  
  constructor(
    private http: HttpClient,
    private dataService: DataService,
    private ApplicationService: ApplicationService
  ) {}

  ngOnInit(): void {
   this.loadUser()
    

    this.fetchData();
    this.dataService.data$.subscribe((data) => {
      console.log("Received data:", data);
      this.applicationArray.push(data);
      console.log(data);
      console.log(
        this.applicationArray +
          'jobpage application data stored in application array'
      );
    });
    // console.log(this.receivedData);
    this.emitData();
  }
  emitData() {
    this.ApplicationService.sendData(this.applicationArray);
  }

  fetchData(): void {
    this.http
      .get<IJob[]>(`${baseurl}/jobs`)
      .pipe(
        catchError((error) => {
          console.error('There was a problem with the fetch operation:', error);
          return throwError(error);
        })
      )
      .subscribe( (data) => {
        console.log(data);
        this.Job = data;
        this.calculateRemainingDays();
        
      });
  }

  calculateRemainingDays(): void {
    const currentDate = new Date();
console.log("Here",this.Job);
console.log("Job array length:", this.Job.length);
    this.Job.forEach((job) => {
      console.log("In Calculate remaining days",job)
      const toTime = new Date(job.toTime);
      const timeDifference = toTime.getTime() - currentDate.getTime();
      this.remainingDays = Math.floor(timeDifference / (1000 * 3600 * 24));
    });
  }
  loadUser(){
    const token=localStorage.getItem("access_token");
   
    const userInfo=token!=null?this.jwtHelperService.decodeToken(token):null;
    console.log("UserInfo",userInfo);

  }
  formatdate(date: Date): string {
    const originalDate = new Date(date);
    const monthNames = [
      'Jan',
      'Feb',
      'Mar',
      'Apr',
      'May',
      'Jun',
      'Jul',
      'Aug',
      'Sep',
      'Oct',
      'Nov',
      'Dec',
    ];
    const day = originalDate.getDate();
    const month = monthNames[originalDate.getMonth()];
    const year = originalDate.getFullYear();
    const formattedDateString = `${day}-${month}-${year}`;
    // console.log(formattedDateString);
    return formattedDateString;
  }
}
