import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatDialogTeacherComponent } from './chat-dialog-teacher.component';

describe('ChatDialogTeacherComponent', () => {
  let component: ChatDialogTeacherComponent;
  let fixture: ComponentFixture<ChatDialogTeacherComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatDialogTeacherComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatDialogTeacherComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
