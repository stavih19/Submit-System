import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeSubmitionExeComponent } from './home-submition-exe.component';

describe('HomeSubmitionExeComponent', () => {
  let component: HomeSubmitionExeComponent;
  let fixture: ComponentFixture<HomeSubmitionExeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HomeSubmitionExeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HomeSubmitionExeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
