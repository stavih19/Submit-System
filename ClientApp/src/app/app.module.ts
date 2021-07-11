import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from './homeApp/login/login.component';
import { HomeComponentComponent } from './homeApp/home-component/home-component.component';
import { NavHomeComponent } from './homeApp/nav-home/nav-home.component';
import { HomeSubmitionExeComponent } from './homeApp/student/home-submition-exe/home-submition-exe.component';
import { BeforeSubmitionExeComponent } from './homeApp/student/before-submition-exe/before-submition-exe.component';

import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { MatSelectModule } from '@angular/material/select';

import { StudentComponent } from './homeApp/student/student.component';
import { CheckerComponent } from './homeApp/checker/checker.component';
import { TeacherComponent } from './homeApp/teacher/teacher.component';
import { ChatDialogComponent } from './homeApp/student/before-submition-exe/chat-dialog/chat-dialog.component';
import { CourseMenuComponent } from './homeApp/teacher/course-menu/course-menu.component';
import { ExeComponent } from './homeApp/teacher/exe/exe.component';
import { ChatReduceComponent } from './homeApp/teacher/exe/chat-reduce/chat-reduce.component';
import { ChatRecudeTeamsComponent } from './homeApp/teacher/exe/chat-recude-teams/chat-recude-teams.component';
import { ChatEditCheckerComponent } from './homeApp/teacher/exe/chat-edit-checker/chat-edit-checker.component';
import { ChatEditTeacherComponent } from './homeApp/teacher/exe/chat-edit-teacher/chat-edit-teacher.component';
import { ChatAutoChecksComponent } from './homeApp/teacher/exe/chat-auto-checks/chat-auto-checks.component';
import { ChatAdvancedChecksComponent } from './homeApp/teacher/exe/chat-auto-checks/chat-advanced-checks/chat-advanced-checks.component';
import { ExtensionComponent } from './homeApp/teacher/extension/extension.component';
import { AppealComponent } from './homeApp/teacher/appeal/appeal.component';
import { AutoCheckComponent } from './homeApp/teacher/auto-check/auto-check.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    LoginComponent,
    HomeComponentComponent,
    NavHomeComponent,
    StudentComponent,
    CheckerComponent,
    TeacherComponent,
    HomeSubmitionExeComponent,
    BeforeSubmitionExeComponent,
    ChatDialogComponent,
    CourseMenuComponent,
    ExeComponent,
    ChatReduceComponent,
    ChatRecudeTeamsComponent,
    ChatEditCheckerComponent,
    ChatEditTeacherComponent,
    ChatAutoChecksComponent,
    ChatAdvancedChecksComponent,
    ExtensionComponent,
    AppealComponent,
    AutoCheckComponent
  ],
  entryComponents: [
    ChatDialogComponent,
    ChatReduceComponent,
    ChatRecudeTeamsComponent,
    ChatEditCheckerComponent,
    ChatEditTeacherComponent,
    ChatAutoChecksComponent,
    ChatAdvancedChecksComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    RouterModule.forRoot([
      { path: 'homeApp/student', component: StudentComponent, pathMatch: 'full' },
      { path: 'homeApp/checker', component: CheckerComponent },
      { path: 'homeApp/teacher', component: TeacherComponent },
    ]),
    FormsModule,
    BrowserAnimationsModule,
    MatInputModule,
    MatButtonModule,
    MatTableModule,
    DragDropModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCheckboxModule,
    ReactiveFormsModule,
    ScrollingModule,
    MatSelectModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
