import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatDeclarationComponent } from './chat-declaration.component';

describe('ChatDeclarationComponent', () => {
  let component: ChatDeclarationComponent;
  let fixture: ComponentFixture<ChatDeclarationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatDeclarationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatDeclarationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
