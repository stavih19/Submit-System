import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatAutoChecksComponent } from './chat-auto-checks.component';

describe('ChatAutoChecksComponent', () => {
  let component: ChatAutoChecksComponent;
  let fixture: ComponentFixture<ChatAutoChecksComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatAutoChecksComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatAutoChecksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
