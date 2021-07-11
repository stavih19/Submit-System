import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatEditCheckerComponent } from './chat-edit-checker.component';

describe('ChatEditCheckerComponent', () => {
  let component: ChatEditCheckerComponent;
  let fixture: ComponentFixture<ChatEditCheckerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatEditCheckerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatEditCheckerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
