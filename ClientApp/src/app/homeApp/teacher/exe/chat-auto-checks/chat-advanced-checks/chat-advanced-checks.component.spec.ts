import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatAdvancedChecksComponent } from './chat-advanced-checks.component';

describe('ChatAdvancedChecksComponent', () => {
  let component: ChatAdvancedChecksComponent;
  let fixture: ComponentFixture<ChatAdvancedChecksComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatAdvancedChecksComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatAdvancedChecksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
