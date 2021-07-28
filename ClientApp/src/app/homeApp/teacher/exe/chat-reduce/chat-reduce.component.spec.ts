import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatReduceComponent } from './chat-reduce.component';

describe('ChatReduceComponent', () => {
  let component: ChatReduceComponent;
  let fixture: ComponentFixture<ChatReduceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatReduceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatReduceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
