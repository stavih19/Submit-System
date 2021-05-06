import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MeanwhileSubmitionExeComponent } from './meanwhile-submition-exe.component';

describe('MeanwhileSubmitionExeComponent', () => {
  let component: MeanwhileSubmitionExeComponent;
  let fixture: ComponentFixture<MeanwhileSubmitionExeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MeanwhileSubmitionExeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MeanwhileSubmitionExeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
