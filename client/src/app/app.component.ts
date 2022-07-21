import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './models/User';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'The Babminton App';
  users:any;

  constructor( private  accountService:AccountService) {}

  ngOnInit(){
  // this.getUsers();
  this.setCurrentUser();
}
  
setCurrentUser(){
  const user:User = JSON.parse(localStorage.getItem('user'));
  this.accountService.setCurrentUser(user);
}

  // getUsers(){
  //   this.http.get('https://localhost:5001/api/Users').subscribe(response=>{
  //     this.users = response;
  //   },error =>{
  //       console.log(error);
  //   }
  //   )
  // }
}