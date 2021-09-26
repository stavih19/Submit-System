import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MainExeComponent } from './main-exe.component';

describe('MainExeComponent', () => {
  let component: MainExeComponent;
  let fixture: ComponentFixture<MainExeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MainExeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MainExeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
