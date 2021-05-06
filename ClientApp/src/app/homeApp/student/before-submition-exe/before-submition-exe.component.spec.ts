import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BeforeSubmitionExeComponent } from './before-submition-exe.component';

describe('BeforeSubmitionExeComponent', () => {
  let component: BeforeSubmitionExeComponent;
  let fixture: ComponentFixture<BeforeSubmitionExeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BeforeSubmitionExeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BeforeSubmitionExeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
