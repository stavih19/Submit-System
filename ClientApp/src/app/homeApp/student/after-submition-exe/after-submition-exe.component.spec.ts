import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AfterSubmitionExeComponent } from './after-submition-exe.component';

describe('AfterSubmitionExeComponent', () => {
  let component: AfterSubmitionExeComponent;
  let fixture: ComponentFixture<AfterSubmitionExeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AfterSubmitionExeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AfterSubmitionExeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
